using UnityEngine;
using UnityEngine.SceneManagement; 
using DG.Tweening; 
using UnityEngine.UI; 
using TMPro; 
using System.Collections.Generic; 

public class LevelButtons : MonoBehaviour
{
    [Header("Tombol UI (Harus Diisi)")]
    [Tooltip("GameObject tombol Next Level")]
    public GameObject nextButton;
    [Tooltip("GameObject tombol Retry")]
    public GameObject retryButton;

    [Header("Pengaturan Animasi")]
    public float initialDelay = 1.0f;
    public float showDuration = 0.3f;
    public Ease showEase = Ease.OutBack;
    
    [Header("Pengaturan Retry")]
    // --- BARU: BOOLEAN KONTROL ---
    [Tooltip("Jika True, map akan berputar 180° sebelum reload. Jika False, langsung reload scene.")]
    public bool animateRetryTransition = true; // Default TRUE
    // --- AKHIR BARU ---
    [Tooltip("Drag script MapContinuousRotation Anda ke sini")]
    public MapContinuousRotation mapRotationController;
    [Tooltip("Durasi rotasi 180 derajat sebelum restart")]
    public float retryRotationDuration = 0.5f;
    [Tooltip("Objects to Hide on Retry")]
    public GameObject[] objectsToHideBeforeRetry;

    [Header("Koneksi Level")]
    public string nextLevelName;

    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
        // ... (Awake logic remains the same) ...
        if (nextButton != null && retryButton != null)
        {
            originalScale = nextButton.transform.localScale;
            
            nextButton.transform.localScale = Vector3.zero;
            retryButton.transform.localScale = Vector3.zero;
            nextButton.SetActive(true);
            retryButton.SetActive(true);
        }
    }

    private void OnEnable()
    {
        ShowButtonsAnimated();
    }

    // --- LOGIKA ANIMASI SHOW (Sama) ---

    public void ShowButtonsAnimated()
    {
        if (nextButton == null || retryButton == null) return;
        
        Sequence showSequence = DOTween.Sequence();

        showSequence.AppendInterval(initialDelay);

        showSequence.Append(
            retryButton.transform.DOScale(originalScale, showDuration)
                       .SetEase(showEase)
        );

        showSequence.Append(
            nextButton.transform.DOScale(originalScale, showDuration)
                       .SetEase(showEase)
        );
    }

    // --- LOGIKA TOMBOL ---

    public void OnNextLevelClicked()
    {
        Time.timeScale = 1f; 
        Debug.Log($"Memuat level berikutnya: {nextLevelName}");
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("Nama Level Berikutnya belum diatur!");
        }
    }

    public void OnRetryClicked()
    {
        // 1. NONAKTIFKAN semua object yang ditunjuk (tetap dilakukan)
        HideObjects();

        // 2. Nonaktifkan interaksi tombol
        SetButtonsInteractable(false);
        
        // --- LOGIKA BARU: Cek Boolean Kontrol ---
        if (animateRetryTransition)
        {
            // Jalankan animasi (LOGIKA LAMA)
            
            if (mapRotationController == null)
            {
                Debug.LogError("Map Rotation Controller belum di-link! Restart langsung.");
                ReloadCurrentScene();
                return;
            }

            Transform mapTransform = mapRotationController.transform;
            Vector3 currentRotation = mapTransform.eulerAngles;
            float targetYRotation = currentRotation.y + 180f; 

            Debug.Log("Memulai rotasi 180 derajat untuk Retry...");

            // Animasi Rotasi, lalu panggil ReloadCurrentScene di OnComplete
            mapTransform.DORotate(
                new Vector3(currentRotation.x, targetYRotation, currentRotation.z),
                retryRotationDuration,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(ReloadCurrentScene); 
        }
        else
        {
            // Jika FALSE, lewati animasi dan langsung reload scene
            Debug.Log("Animasi Retry dimatikan. Langsung memuat ulang scene.");
            ReloadCurrentScene();
        }
        // --- AKHIR LOGIKA BARU ---
    }

    // Fungsi pembantu untuk memuat ulang scene
    private void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Rotasi selesai. Memuat ulang level: {currentSceneName}");
        SceneManager.LoadScene(currentSceneName);
    }
    
    // Fungsi pembantu untuk hide objects
    private void HideObjects()
    {
        if (objectsToHideBeforeRetry == null) return;

        foreach (GameObject obj in objectsToHideBeforeRetry)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        Button nextBtn = nextButton?.GetComponent<Button>();
        Button retryBtn = retryButton?.GetComponent<Button>();
        
        if (nextBtn != null) nextBtn.interactable = interactable;
        if (retryBtn != null) retryBtn.interactable = interactable;
    }
}