using UnityEngine;

public class LevelStarManager : MonoBehaviour
{
    [Header("Pengaturan Level")]
    public string levelNameKey; // Tetap dipakai sebagai "key"

    private int starsCollectedThisRun = 0;

    // Getter ini masih dipakai oleh LevelResultDisplay
    public int GetCurrentRunStars()
    {
        return starsCollectedThisRun;
    }

    void Start()
    {
        // Selalu reset hitungan di awal level
        starsCollectedThisRun = 0;
        
        // --- BARU ---
        // Kita juga reset PlayerPrefs di awal
        // agar hasil ronde lalu tidak terbawa
        PlayerPrefs.SetInt(levelNameKey, 0);
    }

    // Fungsi ini dipanggil oleh skrip 'Star.cs'
    public void CollectStar()
    {
        // --- LOGIKA DIPERMUDAH ---
        // Cukup tambah hitungan. TIDAK ADA PlayerPrefs di sini.
        starsCollectedThisRun++;
        
        // Log sederhana untuk debugging
        Debug.Log($"Bintang diambil! Total sekarang: {starsCollectedThisRun}");
    }

    // --- BARU ---
    // Fungsi ini akan dipanggil oleh FinishZone
    public void SaveCurrentStarsToPrefs()
    {
        // Simpan hasil DARI RONDE INI ke PlayerPrefs
        PlayerPrefs.SetInt(levelNameKey, starsCollectedThisRun);
        PlayerPrefs.Save(); // (Opsional, tapi aman)
        
        Debug.Log($"Hasil akhir disimpan ke '{levelNameKey}': {starsCollectedThisRun} bintang.");
    }
}