using UnityEngine;

public class SceneMusicSetup : MonoBehaviour
{
    [Header("Musik Tema Scene Ini")]
    [Tooltip("Drag file lagu (AudioClip) khusus untuk level/scene ini di sini.")]
    public AudioClip themeMusic;

    void Start()
    {
        // Panggil BGMManager untuk memutar lagu ini
        // Manager akan otomatis mengurus transisi/crossfade-nya
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayMusic(themeMusic);
        }
        else
        {
            // Fallback jika Anda lupa menaruh BGMManager di scene pertama
            // (Hanya untuk testing, sebaiknya selalu start dari Main Menu)
            Debug.LogWarning("BGMManager tidak ditemukan! Pastikan start dari Main Menu.");
        }
    }
}