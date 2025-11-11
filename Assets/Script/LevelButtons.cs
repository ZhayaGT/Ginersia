using UnityEngine;
using UnityEngine.SceneManagement; 
using DG.Tweening; 
using UnityEngine.UI; 
using TMPro; 
using System.Collections.Generic; // Diperlukan untuk List atau IEnumerable

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
    [Tooltip("Drag script MapContinuousRotation Anda ke sini")]
    public MapContinuousRotation mapRotationController;
    [Tooltip("Durasi rotasi 180 derajat sebelum restart")]
    public float retryRotationDuration = 0.5f;
    
    // --- BARU: ARRAY UNTUK HIDE OBJECTS ---
    [Header("Objects to Hide on Retry")]
    [Tooltip("Object yang akan di-hide (SetActive(false)) sebelum rotasi map dimulai. Contoh: Bola, UI Timer, Score Display.")]
    public GameObject[] objectsToHideBeforeRetry;

    [Header("Koneksi Level")]
    public string nextLevelName;

    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
        // Pastikan kedua tombol sudah diisi
        if (nextButton != null && retryButton != null)
        {
            originalScale = nextButton.transform.localScale;
            
            // Atur skala awal ke nol dan pastikan GameObjects aktif
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

    // --- LOGIKA ANIMASI SHOW ---

    public void ShowButtonsAnimated()
    {
        if (nextButton == null || retryButton == null) return;
        
        Sequence showSequence = DOTween.Sequence();

        showSequence.AppendInterval(initialDelay);

        // Animasi Tombol Retry
        showSequence.Append(
            retryButton.transform.DOScale(originalScale, showDuration)
                       .SetEase(showEase)
        );

        // Animasi Tombol Next
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
        // 1. NONAKTIFKAN semua object yang ditunjuk
        HideObjects();

        // 2. Nonaktifkan interaksi tombol
        SetButtonsInteractable(false);
        
        // 3. Cek apakah controller map tersedia
        if (mapRotationController == null)
        {
            Debug.LogError("Map Rotation Controller belum di-link! Restart langsung.");
            ReloadCurrentScene();
            return;
        }

        // 4. Ambil Transform map yang akan diputar
        Transform mapTransform = mapRotationController.transform;

        // 5. Hitung target rotasi 180 derajat tambahan
        Vector3 currentRotation = mapTransform.eulerAngles;
        float targetYRotation = currentRotation.y + 180f; 

        Debug.Log("Memulai rotasi 180 derajat untuk Retry...");

        // 6. Animasi Rotasi
        mapTransform.DORotate(
            new Vector3(currentRotation.x, targetYRotation, currentRotation.z),
            retryRotationDuration,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.InOutSine)
        .OnComplete(ReloadCurrentScene); 
    }

    // Fungsi pembantu untuk memuat ulang scene
    private void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Rotasi selesai. Memuat ulang level: {currentSceneName}");
        SceneManager.LoadScene(currentSceneName);
    }
    
    // --- FUNGSI BARU UNTUK HIDE OBJECTS ---
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