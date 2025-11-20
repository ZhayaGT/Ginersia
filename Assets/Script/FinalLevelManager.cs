using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // WAJIB
using UnityEngine.UI; // WAJIB untuk Image
using System.Collections;

public class FinalLevelManager : MonoBehaviour
{
    [Header("Pengaturan Fade")]
    [Tooltip("Image UI (biasanya warna hitam) yang digunakan untuk fade.")]
    public Image fadeScreen;
    [Tooltip("Nama scene Main Menu.")]
    public string mainMenuSceneName = "MainMenu";
    public float fadeDuration = 1.0f;
    [Tooltip("Jeda (detik) sebelum animasi fade dimulai.")]
    public float preFadeDelay = 0.5f;

    void Start()
    {
        if (fadeScreen != null)
        {
            // Pastikan layar fade ada di scene, dan atur opasitas awal ke 0 (transparan)
            Color color = fadeScreen.color;
            color.a = 0f;
            fadeScreen.color = color;
        }
    }

    // Dipanggil oleh FinalTriggerZone untuk kembali ke Main Menu
    public void LoadMainMenuWithFade()
    {
        if (fadeScreen == null)
        {
            Debug.LogError("Fade Screen belum di-link! Langsung memuat Main Menu.");
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        // Mulai Coroutine untuk mengelola delay dan animasi
        StartCoroutine(FadeAndLoadScene(mainMenuSceneName));
    }

    // Dipanggil oleh FinalTriggerZone untuk keluar
    public void QuitGame()
    {
        Debug.Log("Game Quit Triggered.");
        
        // Catatan: Anda bisa menambahkan fade-out di sini juga jika mau.
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 1. Tunggu delay awal
        yield return new WaitForSeconds(preFadeDelay); 
        
        Debug.Log("Memulai fade-out layar...");

        // 2. Animasi Fade: Opasitas dari 0 ke 1 (penuh)
        fadeScreen.DOFade(1f, fadeDuration)
            .OnComplete(() => {
                // 3. Setelah fade selesai, muat scene
                SceneManager.LoadScene(sceneName);
            });
    }
}