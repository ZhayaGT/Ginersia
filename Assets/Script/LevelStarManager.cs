using UnityEngine;

public class LevelStarManager : MonoBehaviour
{
    [Header("Pengaturan Level")]
    // Ini adalah 'key' untuk PlayerPrefs. 
    // Atur di Inspector (misal: "Level1", "Level2", dst.)
    public string levelNameKey; 

    private int starsCollectedThisRun = 0;

    void Start()
    {
        // Mulai hitungan dari 0 setiap kali level dimulai
        starsCollectedThisRun = 0;
    }

    // Fungsi ini dipanggil oleh skrip 'Star.cs'
    public void CollectStar()
    {
        // Tambah hitungan bintang untuk level ini
        starsCollectedThisRun++;
        
        // --- LOGIKA PENYIMPANAN ---

        // 1. Cek dulu berapa rekor bintang sebelumnya di level ini
        //    Jika belum pernah main, default-nya adalah 0
        int highScor = PlayerPrefs.GetInt(levelNameKey, 0);

        // 2. Jika bintang yang baru kita kumpulkan > rekor lama
        if (starsCollectedThisRun > highScor)
        {
            // Simpan hitungan baru sebagai rekor
            PlayerPrefs.SetInt(levelNameKey, starsCollectedThisRun);
            
            // Sesuai permintaan Anda: Berikan Log
            Debug.Log($"REKOR BARU DISIMPAN! Level '{levelNameKey}' - {starsCollectedThisRun} Bintang.");
        }
        else
        {
            // Sesuai permintaan Anda: Berikan Log
            Debug.Log($"Bintang terkumpul: {starsCollectedThisRun}. (Rekor masih {highScor})");
        }
    }
}