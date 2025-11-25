using UnityEngine;

public class SceneMusicSetup : MonoBehaviour
{
    [Header("Musik Tema Scene Ini")]
    [Tooltip("Masukkan semua layer musik untuk scene ini (misal: Base, Drums, Ambience).")]
    public AudioClip[] themeMusics; // <-- UBAH JADI ARRAY

    void Start()
    {
        if (BGMManager.Instance != null)
        {
            // Kirim array musik ke Manager
            BGMManager.Instance.PlayMusicLayers(themeMusics);
        }
        else
        {
            Debug.LogWarning("BGMManager tidak ditemukan! Pastikan start dari Main Menu.");
        }
    }
}