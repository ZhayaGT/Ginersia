using UnityEngine;
using TMPro; // Hapus baris ini jika Anda tidak pakai TextMeshPro

public class LevelTimer : MonoBehaviour
{
    [Header("Pengaturan Key PlayerPrefs")]
    // Atur di Inspector. Misal: "Level1_Time"
    public string levelTimeKey; 

    [Header("Tampilan UI (Opsional)")]
    public TextMeshProUGUI timerText; 

    private float currentTime = 0f;
    private bool isTimerRunning = false;
    private bool hasTimerStarted = false;

    void Start()
    {
        // Setel ulang timer saat level dimulai
        currentTime = 0f;
        isTimerRunning = false;
        hasTimerStarted = false;
        UpdateTimerText(); // Set teks ke "00:00"
    }

    void Update()
    {
        if (isTimerRunning)
        {
            // Tambahkan waktu setiap frame
            currentTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    // --- Fungsi Publik untuk Dipanggil Skrip Lain ---

    // Dipanggil oleh MapContinuousRotation saat A/D ditekan
    public void StartTimer()
    {
        // Hanya jalankan satu kali
        if (hasTimerStarted) return; 

        hasTimerStarted = true;
        isTimerRunning = true;
        Debug.Log("TIMER DIMULAI!");
    }

    // Dipanggil oleh FinishZone saat level selesai
    public void StopTimer()
    {
        // Hanya stop jika timer sedang berjalan
        if (!isTimerRunning) return; 

        isTimerRunning = false;
        Debug.Log($"TIMER BERHENTI: {currentTime}");

        // Simpan waktu ke PlayerPrefs (Tanpa perbandingan!)
        SaveCurrentTime();
    }

    // --- Logika Internal (Disederhanakan) ---

    private void SaveCurrentTime()
    {
        // --- LOGIKA DISIMPLIFIKASI ---
        // Hanya menyimpan waktu saat ini ke PlayerPrefs
        PlayerPrefs.SetFloat(levelTimeKey, currentTime);
        PlayerPrefs.Save(); // Langsung simpan
        
        Debug.Log($"Waktu penyelesaian disimpan ke '{levelTimeKey}': {currentTime}");
        // -----------------------------
    }

    private void UpdateTimerText()
    {
        // Format waktu jadi 00:00 (Hanya menit dan detik)
        if (timerText != null)
        {
            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}