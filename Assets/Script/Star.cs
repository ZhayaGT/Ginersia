using UnityEngine;

public class Star : MonoBehaviour
{
    // --- BARU ---
    [Header("Audio")]
    public AudioClip collectSound; // Drag SFX koleksi bintang ke sini
    [Range(0f, 1f)]
    public float collectVolume = 1.0f; // Atur volumenya

    private LevelStarManager starManager;

    void Start()
    {
        starManager = FindObjectOfType<LevelStarManager>();
        if (starManager == null)
        {
            Debug.LogError("Tidak ada LevelStarManager di scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (starManager != null)
            {
                starManager.CollectStar();
            }

            // --- BARU ---
            // Mainkan suara di posisi bintang ini
            if (collectSound != null)
            {
                // PlayClipAtPoint akan memutar suara di koordinat dunia (world space)
                // bahkan setelah objek ini hancur.
                AudioSource.PlayClipAtPoint(collectSound, transform.position, collectVolume);
            }

            // Hancurkan GameObject bintang ini (setelah memicu audio)
            Destroy(gameObject);
        }
    }
}