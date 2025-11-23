using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Pengaturan")]
    [Range(0f, 1f)] public float maxVolume = 0.5f; 
    public float crossFadeDuration = 2.0f; 

    private AudioSource sourceA;
    private AudioSource sourceB;
    private bool isSourceAActive = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            sourceA = gameObject.AddComponent<AudioSource>();
            sourceB = gameObject.AddComponent<AudioSource>();
            
            SetupSource(sourceA);
            SetupSource(sourceB);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f; 
    }

    // Fungsi dipanggil saat Scene Baru mulai (SceneMusicSetup)
    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null) return;

        AudioSource activeSource = isSourceAActive ? sourceA : sourceB;
        AudioSource newSource = isSourceAActive ? sourceB : sourceA;

        // Jika lagunya sama, tapi volumenya 0 (karena habis di-fade out), naikkan lagi
        if (activeSource.clip == newClip)
        {
            if (!activeSource.isPlaying) activeSource.Play();
            activeSource.DOFade(maxVolume, crossFadeDuration).SetEase(Ease.Linear);
            return;
        }

        // Setup Source Baru (Fade In)
        newSource.clip = newClip;
        newSource.volume = 0f; // Mulai dari 0
        newSource.Play();
        newSource.DOFade(maxVolume, crossFadeDuration).SetEase(Ease.Linear);

        // Matikan Source Lama (Fade Out)
        if (activeSource.isPlaying)
        {
            activeSource.DOFade(0f, crossFadeDuration).SetEase(Ease.Linear).OnComplete(() => {
                activeSource.Stop();
            });
        }

        isSourceAActive = !isSourceAActive;
    }

    // --- FUNGSI BARU: FADE OUT SAAT DISSOLVE ---
    // Dipanggil oleh LevelButtons saat mau pindah scene
    public void FadeOutCurrentMusic(float duration)
    {
        AudioSource activeSource = isSourceAActive ? sourceA : sourceB;
        
        if (activeSource.isPlaying)
        {
            // Fade volume ke 0 sesuai durasi dissolve
            activeSource.DOFade(0f, duration).SetEase(Ease.Linear);
        }
    }
}