using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Pengaturan")]
    [Range(0f, 1f)] public float maxVolume = 0.5f;
    public float crossFadeDuration = 2.0f;

    // List untuk menyimpan semua AudioSource yang sedang aktif
    private List<AudioSource> activeSources = new List<AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- FUNGSI UTAMA ---
    public void PlayMusicLayers(AudioClip[] newClips)
    {
        if (newClips == null || newClips.Length == 0) return;

        // 1. CEK APAKAH MUSIKNYA SAMA PERSIS?
        if (IsSamePlaylist(newClips))
        {
            Debug.Log("Musik sama dengan scene sebelumnya. Melanjutkan...");
            
            // KITA HANYA PERLU MEMBATALKAN FADE OUT
            // (Karena saat transisi level, volume sempat diturunkan ke 0)
            foreach (AudioSource source in activeSources)
            {
                // Kill tween lama (fade out) dan mulai fade in kembali ke max volume
                source.DOKill(); 
                if (!source.isPlaying) source.Play(); // Jaga-jaga kalau sempat stop
                source.DOFade(maxVolume, crossFadeDuration).SetEase(Ease.Linear);
            }
            return; // KELUAR DARI FUNGSI, JANGAN RESTART LAGU
        }

        // 2. JIKA BEDA, LAKUKAN LOGIKA GANTI LAGU (CROSS-FADE)
        
        // A. Fade Out & Hapus Musik Lama
        // Copy list ke array sementara
        AudioSource[] oldSources = activeSources.ToArray();
        activeSources.Clear(); 

        foreach (AudioSource source in oldSources)
        {
            if (source != null)
            {
                source.DOKill(); // Hentikan animasi volume sebelumnya
                source.DOFade(0f, crossFadeDuration).SetEase(Ease.Linear).OnComplete(() => 
                {
                    source.Stop();
                    Destroy(source); 
                });
            }
        }

        // B. Setup Musik Baru
        foreach (AudioClip clip in newClips)
        {
            if (clip != null)
            {
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.clip = clip;
                newSource.loop = true;
                newSource.playOnAwake = false;
                newSource.volume = 0f; 

                newSource.Play();
                newSource.DOFade(maxVolume, crossFadeDuration).SetEase(Ease.Linear);

                activeSources.Add(newSource);
            }
        }
    }

    public void FadeOutAllMusic(float duration)
    {
        foreach (AudioSource source in activeSources)
        {
            if (source != null)
            {
                source.DOKill();
                source.DOFade(0f, duration).SetEase(Ease.Linear);
            }
        }
    }

    // --- FUNGSI PEMBANTU UNTUK CEK KESAMAAN ---
    private bool IsSamePlaylist(AudioClip[] newClips)
    {
        // Jika jumlah layer beda, pasti beda lagu
        if (activeSources.Count != newClips.Length) return false;

        // Cek satu per satu
        for (int i = 0; i < newClips.Length; i++)
        {
            // Jika activeSource sudah hancur atau clip-nya beda
            if (activeSources[i] == null || activeSources[i].clip != newClips[i])
            {
                return false;
            }
        }

        // Jika semua lolos pengecekan, berarti sama
        return true;
    }
}