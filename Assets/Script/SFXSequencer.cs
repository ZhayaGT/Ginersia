using UnityEngine;
using System.Collections;

public class SFXLooper : MonoBehaviour
{
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

    // AudioSource yang akan digunakan untuk memutar SFX
    private AudioSource audioSource;
    private Coroutine loopCoroutine;

    void Awake()
    {
        // Ambil AudioSource atau tambahkan jika belum ada
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
        StartSFXLoop();
    }

    /// <summary>
    /// Memulai pemutaran SFX secara berulang.
    /// </summary>
    public void StartSFXLoop()
    {
        if (loopingClip == null)
        {
            Debug.LogError("Looping Clip belum diisi!", this);
            return;
        }

        // Pastikan loop sebelumnya berhenti
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
        }
        
        // Mulai coroutine looping
        loopCoroutine = StartCoroutine(ExecuteSFXLoop());
    }

    private IEnumerator ExecuteSFXLoop()
    {
        int currentLoop = 0;

        while (currentLoop < loopCount)
        {
            // 1. Putar SFX
            // Menggunakan PlayOneShot agar tidak mengganggu dirinya sendiri jika ada tumpang tindih
            audioSource.PlayOneShot(loopingClip, volume);

            // 2. Tambah hitungan loop
            currentLoop++;
            
            // 3. Tunggu
            // Kita tunggu durasi clip-nya selesai DITAMBAH waktu delay yang disetel.
            float waitTime = loopingClip.length + delayBetweenLoops;
            yield return new WaitForSeconds(waitTime);
        }
        
        Debug.Log($"Looping SFX selesai setelah {loopCount} kali.");
        loopCoroutine = null; // Reset coroutine reference
    }

    /// <summary>
    /// Menghentikan pemutaran loop SFX.
    /// </summary>
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
        Debug.Log("Looping SFX dihentikan.");
    }
}