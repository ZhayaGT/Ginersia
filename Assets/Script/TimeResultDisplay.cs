using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections; // Diperlukan untuk Coroutine

public class TimeResultDisplay : MonoBehaviour
{
    [Header("Koneksi Level")]
    public LevelTimer levelTimer;

    [Header("Tampilan UI")]
    public TextMeshProUGUI timeText;

    [Header("Pengaturan Animasi")]
    public float initialDelay = 0.5f; // Delay sebelum hitungan dimulai
    public float counterDuration = 1.0f;
    public Ease counterEase = Ease.OutQuad;
    
    [Header("Post-Counter Animasi & Audio")]
    public float finalScaleFactor = 1.3f; // Seberapa besar scale up setelah counter
    public float finalScaleDuration = 0.2f; // Durasi animasi scale up/down
    public AudioClip counterLoopSound;       // Audio yang akan di-loop saat menghitung
    public AudioClip finalDingSound;         // Audio 'Ding!' setelah counter selesai

    private AudioSource audioSource;
    private const float MIN_LOOP_VOLUME = 0.3f; // Volume counter loop

    private void Awake()
    {
        // Ambil komponen AudioSource di panel ini
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Component AudioSource Missing on TimeResultDisplay!");
            return;
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0;
        audioSource.clip = counterLoopSound;
        
        // Mulai mainkan loop sound (tapi volume 0)
        if (counterLoopSound != null)
        {
             audioSource.Play();
        }
    }

    // Fungsi ini dipanggil OTOMATIS saat GameObject ini di-SetAcive(true)
    private void OnEnable()
    {
        // Reset scale teks ke normal (penting!)
        timeText.transform.localScale = Vector3.one; 
        
        // Kita gunakan Coroutine untuk mengontrol DOTween Sequence
        StartCoroutine(StartDisplaySequence());
    }

    private IEnumerator StartDisplaySequence()
    {
        // 1. Validasi
        if (levelTimer == null || timeText == null || audioSource == null) yield break;

        // 2. Delay Awal sebelum memulai hitungan
        yield return new WaitForSeconds(initialDelay);

        // 3. Ambil waktu yang tersimpan
        string fullKey = levelTimer.levelTimeKey;
        float finalTime = PlayerPrefs.GetFloat(fullKey, 0f); 

        // 4. ANIMASI HITUNGAN
        // Buat sekuens DOTween
        Sequence counterSequence = DOTween.Sequence();
        
        // A. Fade In Audio Counter Loop
        counterSequence.Append(audioSource.DOFade(MIN_LOOP_VOLUME, 0.1f)); 
        
        // B. Animasi Hitungan (Core Counter)
        counterSequence.Append(
            DOVirtual.Float(
                0f, 
                finalTime, 
                counterDuration, 
                (value) => {
                    timeText.text = FormatTime(value);
                })
            .SetEase(counterEase)
            .SetId(this)
        );

        // C. Fade Out Audio Counter Loop
        counterSequence.Append(audioSource.DOFade(0f, 0.1f)); 

        // Tunggu hingga seluruh animasi hitungan selesai
        yield return counterSequence.WaitForCompletion(); 
        
        // 5. POST-COUNTER ANIMASI (Scale Up + Ding)
        
        // Putar audio 'Ding!' final
        audioSource.PlayOneShot(finalDingSound, 1f);
    

        // Scale Up dan kembali (Bounce)
        timeText.transform.DOPunchScale(
            Vector3.one * (finalScaleFactor - 1), // Punch Scale Amount
            finalScaleDuration,                    // Durasi
            1,                                    // Vibrato
            0.5f                                  // Elasticity
        );
    }

    /// <summary>
    /// Memformat waktu (float dalam detik) menjadi string MM:SS.ms
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds < 0) timeInSeconds = 0;
        
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);

        // Format string menjadi "MM:SS.ms"
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}