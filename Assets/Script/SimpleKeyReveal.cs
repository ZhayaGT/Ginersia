using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))] // Wajib ada trigger
public class SimpleKeyReveal : MonoBehaviour
{
    [Header("Target Objek")]
    [Tooltip("Daftar objek yang akan dimunculkan saat kunci diambil.")]
    public GameObject[] objectsToReveal;

    [Header("Pengaturan Animasi Muncul")]
    public float fadeDuration = 0.8f; // Lama durasi fade-in per objek
    public float delayBetweenObjects = 0.2f; // Jeda sebelum objek berikutnya muncul
    
    [Header("Efek Kunci")]
    public AudioClip collectSound;
    public float collectVolume = 1.0f;
    public GameObject pickupParticle; // Opsional

    private bool isCollected = false;

    void Awake()
    {
        // --- SETUP AWAL: SEMBUNYIKAN OBJEK ---
        // Kita harus memastikan semua objek target transparansinya 0
        // dan (opsional) dimatikan agar tidak bisa ditabrak sebelum muncul.
        
        if (objectsToReveal != null)
        {
            foreach (GameObject obj in objectsToReveal)
            {
                if (obj != null)
                {
                    // 1. Atur Alpha ke 0 (Transparan)
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color c = sr.color;
                        c.a = 0f;
                        sr.color = c;
                    }

                    // 2. Matikan Objek (agar collider tidak aktif)
                    obj.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectKey();
        }
    }

    private void CollectKey()
    {
        isCollected = true;

        // 1. Mainkan Audio
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);
        }

        // 2. Spawn Partikel (Opsional)
        if (pickupParticle != null)
        {
            Instantiate(pickupParticle, transform.position, Quaternion.identity);
        }

        // 3. ANIMASI MEMUNCULKAN OBJEK (SEQUENCE)
        if (objectsToReveal != null && objectsToReveal.Length > 0)
        {
            Sequence revealSequence = DOTween.Sequence();

            foreach (GameObject obj in objectsToReveal)
            {
                if (obj != null)
                {
                    // A. Aktifkan objek (Collider mulai aktif, tapi masih transparan)
                    revealSequence.AppendCallback(() => {
                        obj.SetActive(true);
                    });

                    // B. Fade In (Alpha 0 ke 1)
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        revealSequence.Append(sr.DOFade(1f, fadeDuration));
                    }

                    // C. Jeda sebelum objek berikutnya (opsional, biar estetik)
                    if (delayBetweenObjects > 0)
                    {
                        revealSequence.AppendInterval(delayBetweenObjects);
                    }
                }
            }
        }

        // 4. Hancurkan Kunci (Animasi Scale Down dulu biar smooth)
        transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
            Destroy(gameObject);
        });
    }
}