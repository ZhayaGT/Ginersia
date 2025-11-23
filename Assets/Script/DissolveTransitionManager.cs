using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Rendering; 
using UnityEngine.Rendering.Universal; 

public class DissolveManager : MonoBehaviour
{
    [Header("Target Dissolve Materials")]
    [Tooltip("Masukkan semua Material yang ingin di-dissolve.")]
    public Material[] materialsToDissolve; 

    [Header("Target Post Processing")]
    public Volume currentLevelVolume;
    public Volume nextLevelVolume;

    [Header("Scene Start: Light Fade In & Out")]
    public Light2D[] sceneLights;
    public float lightFadeInDuration = 1.0f;

    [Header("Pengaturan Shader & Waktu")]
    public string dissolvePropertyName = "_Dissolve_amount";
    public float dissolveDurationPerObject = 1.0f; // Durasi dissolve untuk SATU objek
    [Tooltip("Jeda waktu antar objek dissolve (0 = bareng, 0.2 = berurutan cepat).")]
    public float staggerDelay = 0.2f; // Jeda antar objek

    // Variabel internal
    private float[] initialLightIntensities; 

    void Awake()
    {
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
        // Animasi Lampu Fade In
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

        // Kita gunakan Sequence untuk mengatur timing dissolve material
        Sequence dissolveSequence = DOTween.Sequence();

        // 1. DISSOLVE MATERIAL (SATU PER SATU)
        if (materialsToDissolve != null)
        {
            for (int i = 0; i < materialsToDissolve.Length; i++)
            {
                Material mat = materialsToDissolve[i];
                if (mat != null)
                {
                    // Insert: Memasukkan tween di waktu tertentu dalam timeline
                    // Waktu mulai = index * delay (0, 0.2, 0.4, dst.)
                    float startTime = i * staggerDelay;

                    dissolveSequence.Insert(startTime, 
                        DOVirtual.Float(1f, 0f, dissolveDurationPerObject, (val) => {
                            mat.SetFloat(dissolvePropertyName, val);
                        }).SetEase(Ease.Linear)
                    );
                }
            }
        }

        // 2. TRANSISI GLOBAL (VOLUME & LIGHTS)
        // Kita jalankan ini paralel dengan dissolve material, 
        // tapi durasinya sepanjang total sequence material agar sinkron.
        // Atau bisa juga kita pakai durasi tetap. Di sini saya pakai total durasi sequence.
        
        // Hitung total durasi sequence material (kira-kira)
        float totalDuration = (materialsToDissolve.Length * staggerDelay) + dissolveDurationPerObject;
        
        // Buat tween terpisah untuk global effect (berjalan bersamaan dengan sequence)
        DOVirtual.Float(1f, 0f, totalDuration, (val) => {
            
            // Volume Post-Processing
            if (currentLevelVolume != null) currentLevelVolume.weight = val; 
            if (nextLevelVolume != null) nextLevelVolume.weight = 1f - val; 

            // Lights Fade Out
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

    // --- HELPER ---

    public void ResetMaterials(float value)
    {
        if (materialsToDissolve == null) return;
        foreach (Material mat in materialsToDissolve)
        {
            if (mat != null) mat.SetFloat(dissolvePropertyName, value);
        }
    }
}