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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // --- Setup untuk Suara Rolling (Penting) ---
        // Kita butuh AudioSource kedua khusus untuk loop rolling
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
        // Cek kekuatan benturan
        float impactMagnitude = collision.relativeVelocity.magnitude;

        // Cek 3 hal:
        // 1. Apakah benturan cukup keras? (Velocity Threshold)
        // 2. Apakah jeda waktu sudah lewat? (Cooldown)
        // 3. Apakah kita punya audio clip-nya?
        if (impactMagnitude > minImpactVelocity && 
            Time.time > lastImpactTime + impactCooldown && 
            impactClip != null)
        {
            // Tandai waktu benturan ini
            lastImpactTime = Time.time;

            // --- Trik "Feel": Volume & Pitch Dinamis ---

            // 1. Atur Volume berdasarkan kekuatan benturan
            // Normalisasi kekuatan benturan (0.0 - 1.0)
            float t = Mathf.InverseLerp(minImpactVelocity, maxRollingSpeed, impactMagnitude);
            float dynamicVolume = Mathf.Lerp(0.1f, impactVolume, t);

            // 2. Acak Pitch sedikit agar tidak monoton
            float randomPitch = Random.Range(0.95f, 1.05f);

            // Set pitch SEBELUM memainkan
            audioSource.pitch = randomPitch;
            
            // Mainkan suara "KLAK!" satu kali
            audioSource.PlayOneShot(impactClip, dynamicVolume);
        }
    }
}