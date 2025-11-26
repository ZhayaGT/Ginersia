using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Rendering; 
using UnityEngine.Rendering.Universal; 

[RequireComponent(typeof(AudioSource))] // <-- WAJIB
public class DissolveManager : MonoBehaviour
{
    [Header("Target Dissolve Materials")]
    public Material[] materialsToDissolve; 

    [Header("Target Post Processing")]
    public Volume currentLevelVolume;
    public Volume nextLevelVolume;

    [Header("Scene Start: Light Fade In & Out")]
    public Light2D[] sceneLights;
    public float lightFadeInDuration = 1.0f;

    [Header("Pengaturan Shader & Waktu")]
    public string dissolvePropertyName = "_Dissolve_amount";
    public float dissolveDurationPerObject = 1.0f;
    public float staggerDelay = 0.2f; 

    [Header("Audio Effects")]
    // --- BARU: AUDIO SFX ---
    [Tooltip("SFX yang diputar setiap kali satu objek mulai dissolve.")]
    public AudioClip dissolveSFX;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    // -----------------------

    private float[] initialLightIntensities; 
    private AudioSource audioSource; // Referensi AudioSource

    void Awake()
    {
        // Ambil AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 1. Reset Material
        ResetMaterials(1f);

        // 2. Reset Volume
        if (currentLevelVolume != null) currentLevelVolume.weight = 1f;
        if (nextLevelVolume != null) nextLevelVolume.weight = 0f;

        // 3. Setup Lampu
        if (sceneLights != null && sceneLights.Length > 0)
        {
            initialLightIntensities = new float[sceneLights.Length];
            for (int i = 0; i < sceneLights.Length; i++)
            {
                if (sceneLights[i] != null)
                {
                    initialLightIntensities[i] = sceneLights[i].intensity;
                    sceneLights[i].intensity = 0f;
                }
            }
        }
    }

    void Start()
    {
        if (sceneLights != null && sceneLights.Length > 0)
        {
            for (int i = 0; i < sceneLights.Length; i++)
            {
                if (sceneLights[i] != null)
                {
                    int index = i;
                    float targetIntensity = initialLightIntensities[index];
                    DOVirtual.Float(0f, targetIntensity, lightFadeInDuration, (val) => {
                        if (sceneLights[index] != null) sceneLights[index].intensity = val;
                    }).SetEase(Ease.OutSine);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        ResetMaterials(1f);
    }

    // --- FUNGSI UTAMA (DISSOLVE SATU PER SATU) ---

    public void StartDissolve(Action onComplete)
    {
        Debug.Log($"[DissolveManager] Memulai Dissolve Berurutan...");

        Sequence dissolveSequence = DOTween.Sequence();

        // 1. DISSOLVE MATERIAL & AUDIO (SATU PER SATU)
        if (materialsToDissolve != null)
        {
            for (int i = 0; i < materialsToDissolve.Length; i++)
            {
                Material mat = materialsToDissolve[i];
                if (mat != null)
                {
                    float startTime = i * staggerDelay;

                    // --- BARU: Insert Callback Audio ---
                    // Dipanggil tepat saat animasi dissolve objek ini dimulai
                    dissolveSequence.InsertCallback(startTime, () => {
                        if (dissolveSFX != null)
                        {
                            // Random Pitch sedikit biar tidak monoton (opsional)
                            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                            audioSource.PlayOneShot(dissolveSFX, sfxVolume);
                        }
                    });
                    // -----------------------------------

                    // Insert Animasi Dissolve
                    dissolveSequence.Insert(startTime, 
                        DOVirtual.Float(1f, 0f, dissolveDurationPerObject, (val) => {
                            mat.SetFloat(dissolvePropertyName, val);
                        }).SetEase(Ease.Linear)
                    );
                }
            }
        }

        // 2. TRANSISI GLOBAL (VOLUME & LIGHTS)
        float totalDuration = (materialsToDissolve.Length * staggerDelay) + dissolveDurationPerObject;
        
        DOVirtual.Float(1f, 0f, totalDuration, (val) => {
            
            if (currentLevelVolume != null) currentLevelVolume.weight = val; 
            if (nextLevelVolume != null) nextLevelVolume.weight = 1f - val; 

            if (sceneLights != null)
            {
                for (int j = 0; j < sceneLights.Length; j++)
                {
                    if (sceneLights[j] != null)
                        sceneLights[j].intensity = initialLightIntensities[j] * val;
                }
            }

        }).SetEase(Ease.Linear);

        // 3. SAAT SEMUA SELESAI -> PINDAH SCENE
        dissolveSequence.OnComplete(() => {
            onComplete?.Invoke();
        });
    }

    public void ResetMaterials(float value)
    {
        if (materialsToDissolve == null) return;
        foreach (Material mat in materialsToDissolve)
        {
            if (mat != null) mat.SetFloat(dissolvePropertyName, value);
        }
    }
}