using UnityEngine;

public class FinalTriggerZone : MonoBehaviour
{
    // Tentukan aksi yang akan dilakukan saat trigger ini aktif
    public enum FinalAction { LoadMainMenu, QuitGame }
    
    [Header("Pengaturan Zona")]
    [Tooltip("Pilih aksi yang harus dilakukan trigger ini.")]
    public FinalAction actionType;
    
    [Tooltip("SFX yang dimainkan saat pemain menyentuh trigger.")]
    public AudioClip triggerSound;
    
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;

    private FinalLevelManager manager;
    private bool isActivated = false;

    void Start()
    {
        // Cari Manager di scene
        manager = FindObjectOfType<FinalLevelManager>();
        if (manager == null)
        {
            Debug.LogError("FinalLevelManager tidak ditemukan di scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah itu Player (Bola) dan belum diaktifkan
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            
            // Putar SFX
            if (triggerSound != null)
            {
                AudioSource.PlayClipAtPoint(triggerSound, transform.position, soundVolume);
            }

            // Panggil aksi yang sesuai di Manager
            if (manager != null)
            {
                switch (actionType)
                {
                    case FinalAction.LoadMainMenu:
                        manager.LoadMainMenuWithFade();
                        break;
                    case FinalAction.QuitGame:
                        manager.QuitGame();
                        break;
                }
            }
        }
    }
}