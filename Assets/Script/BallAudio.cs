using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Pastikan ada Rigidbody2D
[RequireComponent(typeof(AudioSource))] // Pastikan ada AudioSource
public class BallAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip impactClip;     // Drag SFX benturan "KLAK!" ke sini
    public AudioClip rollingClip;    // Drag SFX menggelinding "Rooooll" ke sini

    [Header("Pengaturan Benturan (Impact)")]
    [Range(0.1f, 1.0f)]
    public float impactVolume = 0.8f;   // Volume dasar untuk benturan
    public float minImpactVelocity = 1f; // Kecepatan minimum agar suara benturan main
    public float impactCooldown = 0.1f;  // Jeda waktu (detik) antar suara benturan
    
    [Header("Pengaturan Menggelinding (Rolling)")]
    public float minRollingSpeed = 0.1f;   // Kecepatan minimum bola untuk mulai menggelinding
    public float maxRollingSpeed = 8f;     // Kecepatan bola di volume/pitch maksimum
    [Range(0f, 1.0f)]
    public float maxRollingVolume = 0.5f;  // Volume maks suara rolling (agar tidak terlalu keras)
    [Range(0.5f, 1.5f)]
    public float minRollingPitch = 0.8f;   // Pitch saat bola pelan
    [Range(0.5f, 1.5f)]
    public float maxRollingPitch = 1.2f;   // Pitch saat bola cepat

    // Komponen
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private AudioSource rollingAudioSource; // AudioSource terpisah untuk looping

    // Variabel internal
    private float lastImpactTime;

    private CameraShaker cameraShaker;

    // --- BARU: PENGATURAN SCREENSHAKE ---
    [Header("Pengaturan ScreenShake")]
    public float shakeDuration = 0.1f; // Durasi getaran
    public float minShakeStrength = 0.1f; // Kekuatan getar saat benturan pelan
    public float maxShakeStrength = 0.5f; // Kekuatan getar saat benturan keras

    [System.Obsolete]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        cameraShaker = FindObjectOfType<CameraShaker>();
        if (cameraShaker == null)
        {
            Debug.LogWarning("CameraShaker tidak ditemukan di scene!");
        }
        // agar tidak bentrok dengan suara impact (one-shot)
        rollingAudioSource = gameObject.AddComponent<AudioSource>();
        rollingAudioSource.clip = rollingClip;
        rollingAudioSource.loop = true; // Suara rolling harus loop
        rollingAudioSource.playOnAwake = false;
        rollingAudioSource.volume = 0; // Mulai dari mati
        rollingAudioSource.Play(); // Langsung mainkan, tapi volume 0
    }

    void Update()
    {
        HandleRollingSound();
    }

    // --- 1. Logika Suara Menggelinding ---
    void HandleRollingSound()
    {
        // Dapatkan kecepatan bola saat ini
        float speed = rb.linearVelocity.magnitude;

        // Cek apakah bola bergerak cukup cepat
        if (speed > minRollingSpeed)
        {
            // 't' adalah nilai antara 0.0 - 1.0 berdasarkan kecepatan bola
            float t = Mathf.InverseLerp(minRollingSpeed, maxRollingSpeed, speed);

            // Atur Volume dan Pitch berdasarkan 't'
            rollingAudioSource.volume = Mathf.Lerp(0, maxRollingVolume, t);
            rollingAudioSource.pitch = Mathf.Lerp(minRollingSpeed, maxRollingPitch, t);
        }
        else
        {
            // Jika bola terlalu pelan, matikan suaranya
            rollingAudioSource.volume = 0;
        }
    }

    // --- 2. Logika Suara Benturan ---
    // Fungsi ini dipanggil oleh fisika Unity
    void OnCollisionEnter2D(Collision2D collision)
    {
        float impactMagnitude = collision.relativeVelocity.magnitude;

        // Cek jika benturan cukup keras DAN cooldown sudah lewat
        if (impactMagnitude > minImpactVelocity && 
            Time.time > lastImpactTime + impactCooldown)
        {
            // Tandai waktu benturan
            lastImpactTime = Time.time;
            
            // --- Logika Audio (dari skrip lama) ---
            if (impactClip != null)
            {
                float t = Mathf.InverseLerp(minImpactVelocity, maxRollingSpeed, impactMagnitude);
                float dynamicVolume = Mathf.Lerp(0.1f, impactVolume, t);
                float randomPitch = Random.Range(0.95f, 1.05f);
                audioSource.pitch = randomPitch;
                audioSource.PlayOneShot(impactClip, dynamicVolume);
            }

            // --- BARU: LOGIKA SCREENSHAKE ---
            if (cameraShaker != null)
            {
                // Gunakan 't' yang sama dari kalkulasi audio
                float t = Mathf.InverseLerp(minImpactVelocity, maxRollingSpeed, impactMagnitude);
                
                // Hitung kekuatan shake berdasarkan kekuatan benturan
                float dynamicShakeStrength = Mathf.Lerp(minShakeStrength, maxShakeStrength, t);
                
                // Panggil fungsi shake di kamera
                cameraShaker.TriggerShake(dynamicShakeStrength, shakeDuration);
            }
        }
    }
}