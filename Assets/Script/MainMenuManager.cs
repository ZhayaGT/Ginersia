using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MainMenuManager : MonoBehaviour
{
    [Header("Koneksi UI")]
    public Image titleImage; 
    public GameObject startButton;
    public GameObject exitButton;
    
    [Header("Koneksi Sistem")]
    [Tooltip("Drag GameObject yang memiliki script DissolveManager ke sini.")]
    public DissolveManager dissolveManager; 

    [Header("Pengaturan Animasi")]
    public string startSceneName = "Level1"; 
    public float initialDelay = 0.5f;
    public float animDuration = 0.3f; 
    public Ease animEase = Ease.OutBack;
    
    [Header("Pengaturan Hide UI Menu")]
    [Tooltip("Objek UI Menu yang akan LANGSUNG di-hide (SetActive false) sebelum dissolve.")]
    public GameObject[] menuObjectsToHide; // Ganti nama variabel biar jelas

    [Header("Pengaturan Audio")]
    public AudioClip popInSound;
    public AudioClip clickSound; 
    [Range(0f, 1f)] public float popInVolume = 0.8f;
    [Range(0f, 1f)] public float clickVolume = 1.0f; 
    
    private Vector3 originalScale = Vector3.one;
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    
    void Start()
    {
        DOTween.Init();

        if (titleImage != null) originalScale = titleImage.transform.localScale;
        
        if (titleImage != null) titleImage.transform.localScale = Vector3.zero;
        if (startButton != null) startButton.transform.localScale = Vector3.zero;
        if (exitButton != null) exitButton.transform.localScale = Vector3.zero;

        StartCoroutine(ShowMenuSequence());
    }

    private IEnumerator ShowMenuSequence()
    {
        System.Action playPopSFX = () => {
            if (popInSound != null) audioSource.PlayOneShot(popInSound, popInVolume);
        };
        
        yield return new WaitForSeconds(initialDelay);

        if (titleImage != null)
        {
            titleImage.transform.DOScale(originalScale, animDuration).SetEase(animEase);
            playPopSFX(); 
            yield return new WaitForSeconds(animDuration);
        }

        if (startButton != null)
        {
            startButton.transform.DOScale(originalScale, animDuration).SetEase(animEase);
            playPopSFX(); 
            yield return new WaitForSeconds(animDuration);
        }

        if (exitButton != null)
        {
            exitButton.transform.DOScale(originalScale, animDuration).SetEase(animEase);
            playPopSFX();
        }
    }

    // --- FUNGSI START BUTTON (MODIFIKASI) ---

    public void OnStartClicked()
    {
        if (string.IsNullOrEmpty(startSceneName))
        {
            Debug.LogError("Nama Scene Start belum diatur!");
            return;
        }
        
        // Matikan interaksi tombol
        if (startButton != null) startButton.GetComponent<Button>().interactable = false;
        if (exitButton != null) exitButton.GetComponent<Button>().interactable = false;
        
        // Mulai proses transisi (Tanpa Coroutine kali ini, karena langsung hide)
        StartGameTransitionDirect();
    }

    private void StartGameTransitionDirect()
    {
        // 1. Mainkan SFX Klik
        if (clickSound != null) audioSource.PlayOneShot(clickSound, clickVolume);

        // 2. LANGSUNG HIDE SEMUA OBJEK MENU
        // Tidak perlu Sequence atau Fade, langsung matikan saja
        if (menuObjectsToHide != null)
        {
            foreach (GameObject obj in menuObjectsToHide)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        // 3. PANGGIL DISSOLVE MANAGER
        if (dissolveManager != null)
        {
            Debug.Log("Memulai Dissolve Transisi...");
            
            // Panggil dissolve segera setelah UI hilang
            dissolveManager.StartDissolve(() => {
                SceneManager.LoadScene(startSceneName);
            });
        }
        else
        {
            Debug.LogWarning("DissolveManager tidak di-link! Pindah scene langsung.");
            SceneManager.LoadScene(startSceneName);
        }
    }

    public void OnExitClicked()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound, clickVolume);
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}