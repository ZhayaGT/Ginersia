using UnityEngine;
using UnityEngine.SceneManagement; 
using DG.Tweening; 
using UnityEngine.UI; 
using TMPro; 

public class LevelButtons : MonoBehaviour
{
    [Header("Tombol UI")]
    public GameObject nextButton;
    public GameObject retryButton;

    [Header("Animasi UI")]
    public float initialDelay = 1.0f;
    public float showDuration = 0.3f;
    public Ease showEase = Ease.OutBack;
    
    // --- PERUBAHAN: REFERENSI KE SCRIPT BARU ---
    [Header("Koneksi Sistem")]
    public DissolveManager dissolveManager; // Drag script baru ke sini
    public MapContinuousRotation mapRotationController;

    [Header("Pengaturan Retry")]
    public bool animateRetryTransition = true; 
    public float rotationDuration = 1f;
    public GameObject[] objectsToHideBeforeRetry;

    [Header("Koneksi Level")]
    public string nextLevelName;

    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
        // Setup Tombol
        if (nextButton != null && retryButton != null)
        {
            originalScale = nextButton.transform.localScale;
            nextButton.transform.localScale = Vector3.zero;
            retryButton.transform.localScale = Vector3.zero;
            nextButton.SetActive(true);
            retryButton.SetActive(true);
        }
        
        // Catatan: Reset material sekarang diurus oleh DissolveManager.Awake()
    }

    private void OnEnable()
    {
        ShowButtonsAnimated();
    }

    public void ShowButtonsAnimated()
    {
        if (nextButton == null || retryButton == null) return;
        
        Sequence showSequence = DOTween.Sequence();
        showSequence.AppendInterval(initialDelay);
        showSequence.Append(retryButton.transform.DOScale(originalScale, showDuration).SetEase(showEase));
        showSequence.Append(nextButton.transform.DOScale(originalScale, showDuration).SetEase(showEase));
    }

    // --- LOGIKA TOMBOL NEXT (PANGGIL DISSOLVE MANAGER) ---

    public void OnNextLevelClicked()
    {
        HideObjects();
        SetButtonsInteractable(false);

        if (string.IsNullOrEmpty(nextLevelName))
        {
            Debug.LogError("Nama Level Berikutnya belum diatur!");
            return;
        }

        float musicFadeDuration = 1.5f; // Default, sesuaikan dengan dissolveDuration Anda
        if (dissolveManager != null) musicFadeDuration = dissolveManager.dissolveDurationPerObject;

        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.FadeOutCurrentMusic(musicFadeDuration);
        }

        // 1. Putar Map Dulu (Jika ada controller)
        if (mapRotationController != null)
        {
            Debug.Log("Memutar map kembali ke 0...");
            mapRotationController.transform.DORotate(Vector3.zero, rotationDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => {
                    // 2. Setelah putar selesai, panggil Dissolve Manager
                    CallDissolveAndLoad();
                });
        }
        else
        {
            // Jika tidak ada map controller, langsung dissolve
            CallDissolveAndLoad();
        }
    }

    private void CallDissolveAndLoad()
    {
        if (dissolveManager != null)
        {
            // Panggil fungsi di script sebelah, dan berikan aksi LoadScene
            dissolveManager.StartDissolve(() => {
                SceneManager.LoadScene(nextLevelName);
            });
        }
        else
        {
            Debug.LogWarning("DissolveManager belum di-link! Pindah scene instan.");
            SceneManager.LoadScene(nextLevelName);
        }
    }

    // --- LOGIKA TOMBOL RETRY ---

    public void OnRetryClicked()
    {
        HideObjects();
        SetButtonsInteractable(false);
        
        if (animateRetryTransition)
        {
            if (mapRotationController == null)
            {
                ReloadCurrentScene();
                return;
            }

            Transform mapTransform = mapRotationController.transform;
            Vector3 currentRotation = mapTransform.eulerAngles;
            float targetYRotation = currentRotation.y + 180f; 

            mapTransform.DORotate(
                new Vector3(currentRotation.x, targetYRotation, currentRotation.z),
                rotationDuration,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(ReloadCurrentScene); 
        }
        else
        {
            ReloadCurrentScene();
        }
    }

    private void ReloadCurrentScene()
    {
        // Pastikan material di-reset sebelum reload agar visual aman
        if (dissolveManager != null) dissolveManager.ResetMaterials(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void HideObjects()
    {
        if (objectsToHideBeforeRetry == null) return;
        foreach (GameObject obj in objectsToHideBeforeRetry)
        {
            if (obj != null) obj.SetActive(false);
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