using UnityEngine;

// --- BARU ---
// Pastikan ada AudioSource di GameObject ini
[RequireComponent(typeof(AudioSource))] 
public class FinishZone : MonoBehaviour
{
    [Header("Pengaturan Finish")]
    [SerializeField] private Transform newParentOnFinish;
    [SerializeField] private MapContinuousRotation mapRotationScript;

    // --- BARU ---
    [Header("Audio")]
    public AudioClip finishJingle; // Drag SFX jingle kemenangan ke sini
    [SerializeField] private LevelTimer levelTimer; // Drag LevelTimer ke sini
    [SerializeField] private LevelStarManager starManager;

    private const string PLAYER_TAG = "Player";
    private bool hasFinished = false;
    private AudioSource audioSource; // Referensi ke AudioSource

    // --- BARU ---
    void Start()
    {
        // Ambil komponen AudioSource
        audioSource = GetComponent<AudioSource>();
        // Pastikan tidak 'Play On Awake'
        audioSource.playOnAwake = false; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && !hasFinished)
        {
            hasFinished = true;
            Debug.Log("LEVEL SELESAI! Bola telah mencapai finish.");

            // --- BARU ---
            // Mainkan suara finish
            if (finishJingle != null)
            {
                audioSource.PlayOneShot(finishJingle);
            }
            
            if (levelTimer != null)
            {
                levelTimer.StopTimer();
            }

            if (starManager != null)
            {
                starManager.SaveCurrentStarsToPrefs();
            }
            
            Rigidbody2D ballRb = other.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.isKinematic = true;
            }

            if (newParentOnFinish != null)
            {
                other.transform.SetParent(newParentOnFinish);
            }

            if (mapRotationScript != null)
            {
                mapRotationScript.StartResetRotation();
            }
        }
    }
}