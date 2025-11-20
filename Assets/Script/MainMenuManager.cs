using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MainMenuManager : MonoBehaviour
{
    [Header("Koneksi UI")]
    [Tooltip("Gambar Judul/Logo")]
    public Image titleImage; 
    [Tooltip("GameObject tombol Start")]
    public GameObject startButton;
    [Tooltip("GameObject tombol Exit")]
    public GameObject exitButton;
    
    [Header("Pengaturan Fade Out Awal")]
    [Tooltip("Objek yang akan di-fade out (misalnya Background atau UI yang sudah di-fade in).")]
    public GameObject[] objectsToFadeOut;
    public float fadeOutDuration = 0.5f;

    // --- BARU: Objek yang diaktifkan saat Start ---
    [Header("Transisi Start Button")]
    [Tooltip("Objek yang akan diaktifkan (SetActive(true)) saat tombol Start ditekan.")]
    public GameObject objectToShowOnStart;
    // --- AKHIR BARU ---
    
    [Header("Pengaturan Animasi")]
    [Tooltip("Nama Scene yang akan dimuat saat tombol Start diklik")]
    public string startSceneName = "Level1"; 
    public float initialDelay = 0.5f;
    public float animDuration = 0.3f; 
    public Ease animEase = Ease.OutBack;
    
    [Header("Pengaturan Audio")]
    [Tooltip("SFX yang dimainkan saat setiap elemen muncul (pop-in)")]
    public AudioClip popInSound;
    [Tooltip("SFX yang dimainkan saat menekan tombol Start/Exit")]
    public AudioClip clickSound; 
    [Range(0f, 1f)]
    public float popInVolume = 0.8f;
    [Range(0f, 1f)]
    public float clickVolume = 1.0f; 
    
    private Vector3 originalScale = Vector3.one;
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }
    
    void Start()
    {
        DOTween.Init();

        if (titleImage != null) originalScale = titleImage.transform.localScale;
        
        // Atur skala awal ke nol
        if (titleImage != null) titleImage.transform.localScale = Vector3.zero;
        if (startButton != null) startButton.transform.localScale = Vector3.zero;
        if (exitButton != null) exitButton.transform.localScale = Vector3.zero;

        // Mulai sekuens animasi
        StartCoroutine(ShowMenuSequence());
    }

    private IEnumerator ShowMenuSequence()
    {
        // ... (Logika ShowMenuSequence tetap sama untuk Pop-In awal) ...
        
        // Fungsi pembantu untuk memutar SFX Pop-In
        System.Action playPopSFX = () => {
            if (popInSound != null)
            {
                audioSource.PlayOneShot(popInSound, popInVolume);
            }
        };
        
        yield return new WaitForSeconds(initialDelay);

        // 1. Animasi Gambar Judul
        if (titleImage != null)
        {
            titleImage.transform.DOScale(originalScale, animDuration).SetEase(animEase);
            playPopSFX(); 
            yield return new WaitForSeconds(animDuration);
        }

        // 2. Animasi Tombol Start
        if (startButton != null)
        {
            startButton.transform.DOScale(originalScale, animDuration).SetEase(animEase);
            playPopSFX(); 
            yield return new WaitForSeconds(animDuration);
        }

        // 3. Animasi Tombol Exit
        if (exitButton != null)
        {
            exitButton.transform.DOScale(originalScale, animDuration).SetEase(animEase);
            playPopSFX();
            // Tidak perlu menunggu karena ini item terakhir
        }
    }

    // --- FUNGSI ONCLICK MODIFIKASI ---

    public void OnStartClicked()
    {
        // Cek dasar
        if (string.IsNullOrEmpty(startSceneName))
        {
            Debug.LogError("Nama Scene Start belum diatur di Inspector!");
            return;
        }
        
        // Nonaktifkan tombol agar tidak bisa diklik dua kali
        if (startButton != null) startButton.GetComponent<Button>().interactable = false;
        if (exitButton != null) exitButton.GetComponent<Button>().interactable = false;
        
        // Mulai sekuens transisi
        StartCoroutine(StartGameTransitionSequence());
    }

    private IEnumerator StartGameTransitionSequence()
    {
        // 1. Mainkan SFX Klik
        if (clickSound != null) audioSource.PlayOneShot(clickSound, clickVolume);

        // 2. Siapkan dan jalankan animasi Fade Out
        Sequence fadeSequence = DOTween.Sequence();
        
        // Tambahkan Fade Out untuk semua objek yang ditandai
        foreach (GameObject obj in objectsToFadeOut)
        {
            if (obj != null)
            {
                CanvasGroup cg = obj.GetComponent<CanvasGroup>();
                Image img = obj.GetComponent<Image>();
                
                if (cg != null)
                {
                    fadeSequence.Join(cg.DOFade(0f, fadeOutDuration));
                }
                else if (img != null)
                {
                    // Pastikan Image tidak null sebelum di-fade
                    fadeSequence.Join(img.DOFade(0f, fadeOutDuration));
                }
                // Catatan: Jika objek tidak memiliki CanvasGroup atau Image, ia tidak akan di-fade.
            }
        }
        
        // 3. Tambahkan Callback untuk MUNCULKAN OBJEK (SetActive(true))
        fadeSequence.AppendCallback(() => {
            if (objectToShowOnStart != null)
            {
                objectToShowOnStart.SetActive(true);
                Debug.Log($"Objek '{objectToShowOnStart.name}' diaktifkan saat transisi.");
            }
        });

        // 4. Tunggu hingga seluruh fade out selesai
        yield return fadeSequence.WaitForCompletion();

        // 5. Muat Scene
        Debug.Log($"Fade selesai. Memuat Scene: {startSceneName}");
        SceneManager.LoadScene(startSceneName);
    }


    public void OnExitClicked()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound, clickVolume);

        Debug.Log("Keluar dari Game.");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}