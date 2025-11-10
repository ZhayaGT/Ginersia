using UnityEngine;
using DG.Tweening; // <-- WAJIB
using UnityEngine.UI;
using System.Collections;

// --- BARU ---
// Otomatis tambahkan AudioSource ke panel ini
[RequireComponent(typeof(AudioSource))]
public class LevelResultDisplay : MonoBehaviour
{
    [Header("Koneksi Level")]
    public LevelStarManager starManager; 

    [Header("Tampilan UI")]
    public GameObject[] uiStars; 

    [Header("Animasi 'Stamp'")]
    public float stampDuration = 0.25f;
    public float stampStartScale = 1.8f;
    public float delayPerStar = 0.2f;
    public Ease stampEase = Ease.OutBack;
    
    // --- BARU: PENGATURAN AUDIO ---
    [Header("Audio 'Stamp'")]
    public AudioClip stampSound; // Drag SFX "cap" Anda ke sini
    [Range(0f, 1f)]
    public float stampVolume = 1.0f;

    private AudioSource audioSource; // Referensi untuk AudioSource

    // --- BARU ---
    // Gunakan Awake() untuk mengambil komponen
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Pastikan tidak play on awake
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        ShowStarResultsAnimated();
    }

    private void ShowStarResultsAnimated()
    {
        if (starManager == null)
        {
            Debug.LogError("LevelStarManager belum di-link di LevelResultDisplay!", this);
            return;
        }

        // --- Perbaikan Bug Krusial ---
        int starsCollected = starManager.GetCurrentRunStars(); 
        
        Debug.Log($"Menampilkan hasil animasi: {starsCollected} bintang...");

        // 1. Matikan semua bintang dulu (Reset)
        foreach (GameObject star in uiStars)
        {
            if (star != null)
            {
                star.SetActive(false);
                star.transform.localScale = Vector3.one;
            }
        }

        // 2. Buat Sekuens DOTween baru
        Sequence stampSequence = DOTween.Sequence();

        // 3. Loop sebanyak bintang yang didapat
        for (int i = 0; i < starsCollected; i++)
        {
            if (i >= uiStars.Length || uiStars[i] == null) continue;

            GameObject starGO = uiStars[i];
            CanvasGroup cg = starGO.GetComponent<CanvasGroup>();
            Image img = starGO.GetComponent<Image>();

            // --- LOGIKA "CAP" ---

            // A. Callback untuk MENYIAPKAN bintang
            stampSequence.AppendCallback(() => {
                starGO.SetActive(true); 
                starGO.transform.localScale = Vector3.one * stampStartScale; 

                if (cg != null) cg.alpha = 0f;
                else if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                
                // --- BARU: MAINKAN SUARA "CAP" ---
                if (stampSound != null)
                {
                    audioSource.PlayOneShot(stampSound, stampVolume);
                }
                // --- AKHIR BARU ---
            });

            // B. Animasi UTAMA (Scale & Fade)
            stampSequence.Append(
                starGO.transform.DOScale(1f, stampDuration)
                    .SetEase(stampEase) 
            );

            if (cg != null)
            {
                stampSequence.Join(cg.DOFade(1f, stampDuration * 0.75f)); 
            }
            else if (img != null)
            {
                stampSequence.Join(img.DOFade(1f, stampDuration * 0.75f));
            }

            // C. JEDA sebelum bintang berikutnya
            stampSequence.AppendInterval(delayPerStar);
        }
    }
}