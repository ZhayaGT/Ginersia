using UnityEngine;
using DG.Tweening; // <-- WAJIB TAMBAHKAN INI

public class Star : MonoBehaviour
{
    public AudioClip collectSound; 
    [Range(0f, 1f)]
    public float collectVolume = 1.0f; 
    
    // --- BARU: Pengaturan Animasi Koleksi ---
    [Header("Animasi Koleksi")]
    public float animDuration = 0.5f; 
    public float moveUpAmount = 0.5f; 
    public Ease scaleEase = Ease.InBack; 
    public Ease moveEase = Ease.OutQuad; 
    
    // --- BARU: Pengaturan Animasi Awal ---
    [Header("Animasi Pop-In Awal")]
    public float initialPopDuration = 0.5f; // Durasi animasi pop-in
    public float initialPopScale = 1.2f;    // Skala maksimum saat pop-in
    public Ease initialPopEase = Ease.OutBack; // Efek pantulan untuk pop-in
    public AudioClip initialPopSound;         // SFX yang akan dimainkan saat pop-in

    private LevelStarManager starManager;
    private bool isCollected = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        // --- BARU: Panggil fungsi Pop-In saat objek pertama kali muncul ---
        // Kita panggil di Awake() agar terjadi sebelum Start()
        StartInitialPopIn();
    }

    void Start()
    {
        starManager = FindObjectOfType<LevelStarManager>();
        if (starManager == null)
        {
            Debug.LogError("Tidak ada LevelStarManager di scene!");
        }

        // Ambil referensi komponen
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    
    // --- FUNGSI BARU: Animasi Pop-In di Awal ---
    private void StartInitialPopIn()
    {
        // 1. Atur skala awal ke nol
        transform.localScale = Vector3.zero;

        // 2. Buat sekuens DOTween
        Sequence initialSequence = DOTween.Sequence();
        
        // 3. Tambahkan animasi Scale
        initialSequence.Append(
            transform.DOScale(initialPopScale, initialPopDuration) // Scale ke ukuran besar (pop)
                     .SetEase(initialPopEase)
        );
        
        // 4. Tambahkan animasi Scale kembali ke ukuran normal (1)
        initialSequence.Append(
            transform.DOScale(1f, initialPopDuration * 0.5f) // Kembali ke 1.0 lebih cepat
                     .SetEase(Ease.OutQuad)
        );

        // 5. Tambahkan callback untuk audio (diputar saat scale-up dimulai)
        initialSequence.OnStart(() => {
            if (initialPopSound != null)
            {
                AudioSource.PlayClipAtPoint(initialPopSound, transform.position, collectVolume);
            }
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true; 

            // Matikan collider
            if (col != null)
            {
                col.enabled = false;
            }

            // 2. Beri tahu manager
            if (starManager != null)
            {
                starManager.CollectStar();
            }

            // 3. Mainkan suara koleksi
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);
            }

            // Buat sekuens animasi koleksi (tetap sama)
            Sequence collectSequence = DOTween.Sequence();
            collectSequence.Join(
                transform.DOMoveY(transform.position.y + moveUpAmount, animDuration)
                         .SetEase(moveEase)
            );
            collectSequence.Join(
                transform.DOScale(0f, animDuration)
                         .SetEase(scaleEase)
            );
            if (spriteRenderer != null)
            {
                collectSequence.Join(
                    spriteRenderer.DOFade(0f, animDuration)
                                  .SetEase(moveEase)
                );
            }

            // 4. Hancurkan objek SETELAH semua animasi selesai
            collectSequence.OnComplete(() => {
                Destroy(gameObject);
            });
        }
    }
}