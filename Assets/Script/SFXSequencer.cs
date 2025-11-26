using UnityEngine;
using System.Collections;

public class SFXLooper : MonoBehaviour
{
    [Header("Pengaturan Awal")]
    [Tooltip("Waktu tunggu sebelum looping dimulai.")]
    public float startDelay = 1.0f; // Default 1 detik sesuai request

    [Header("Pengaturan Loop")]
    [Tooltip("SFX yang akan diputar berulang.")]
    public AudioClip loopingClip;
    
    [Tooltip("Berapa kali SFX akan diputar.")]
    public int loopCount = 5;

    [Tooltip("Jeda (delay) dalam detik di antara setiap putaran.")]
    public float delayBetweenLoops = 0.1f;
    
    [Tooltip("Volume saat memutar clip ini (0.0 - 1.0).")]
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource audioSource;
    private Coroutine loopCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    void Start()
    {
        // Jika Anda ingin ini otomatis jalan saat scene mulai
        StartSFXLoop();
    }

    public void StartSFXLoop()
    {
        if (loopingClip == null)
        {
            Debug.LogError("Looping Clip belum diisi!", this);
            return;
        }

        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
        }
        
        loopCoroutine = StartCoroutine(ExecuteSFXLoop());
    }

    private IEnumerator ExecuteSFXLoop()
    {
        // --- BARU: TUNGGU DELAY AWAL ---
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        // -------------------------------

        int currentLoop = 0;

        while (currentLoop < loopCount)
        {
            // 1. Putar SFX
            audioSource.PlayOneShot(loopingClip, volume);

            // 2. Tambah hitungan loop
            currentLoop++;
            
            // 3. Tunggu durasi clip + interval
            float waitTime = loopingClip.length + delayBetweenLoops;
            yield return new WaitForSeconds(waitTime);
        }
        
        Debug.Log($"Looping SFX selesai setelah {loopCount} kali.");
        loopCoroutine = null; 
    }

    public void StopSFXLoop()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}