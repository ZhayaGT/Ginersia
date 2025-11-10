using UnityEngine;
using DG.Tweening; // <-- WAJIB TAMBAHKAN INI

public class Star : MonoBehaviour
{
    public AudioClip collectSound; 
    [Range(0f, 1f)]
    public float collectVolume = 1.0f; 

    // --- BARU: Pengaturan Animasi ---
    [Header("Animasi Koleksi")]
    public float animDuration = 0.5f;   // Total durasi animasi
    public float moveUpAmount = 0.5f;   // Jarak bintang bergerak ke atas
    public Ease scaleEase = Ease.InBack; // Tipe ease untuk mengecil (InBack bagus)
    public Ease moveEase = Ease.OutQuad;   // Tipe ease untuk bergerak & fade

    private LevelStarManager starManager;
    private bool isCollected = false;

    // --- BARU: Referensi Komponen ---
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Start()
    {
        starManager = FindObjectOfType<LevelStarManager>();
        if (starManager == null)
        {
            Debug.LogError("Tidak ada LevelStarManager di scene!");
        }

        // --- BARU: Ambil referensi komponen ---
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek 3 hal:
        // 1. Apakah yang menyentuh adalah Player?
        // 2. Apakah bintang ini BELUM diambil?
        if (other.CompareTag("Player") && !isCollected)
        {
            // 1. Langsung tandai sudah diambil!
            isCollected = true; 

            // --- BARU: Nonaktifkan collider ---
            // Agar tidak memicu trigger lagi selagi animasi berjalan
            if (col != null)
            {
                col.enabled = false;
            }

            // 2. Beri tahu manager
            if (starManager != null)
            {
                starManager.CollectStar();
            }

            // 3. Mainkan suara
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);
            }

            // --- BARU: Hapus "Destroy(gameObject);" ---
            // ganti dengan sekuens animasi DOTween

            // Buat sekuens baru
            Sequence collectSequence = DOTween.Sequence();

            // Tambahkan animasi bergerak ke atas
            collectSequence.Join(
                transform.DOMoveY(transform.position.y + moveUpAmount, animDuration)
                         .SetEase(moveEase)
            );

            // Tambahkan animasi mengecil (scale ke 0)
            collectSequence.Join(
                transform.DOScale(0f, animDuration)
                         .SetEase(scaleEase)
            );

            // Tambahkan animasi fade out (hilang perlahan)
            if (spriteRenderer != null)
            {
                collectSequence.Join(
                    spriteRenderer.DOFade(0f, animDuration)
                                  .SetEase(moveEase) // Bisa pakai ease yg sama
                );
            }

            // 4. Hancurkan objek SETELAH semua animasi selesai
            collectSequence.OnComplete(() => {
                Destroy(gameObject);
            });
        }
    }
}