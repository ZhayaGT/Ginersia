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

    // --- FUNGSI UTAMA: Memutar Array Musik ---
    public void PlayMusicLayers(AudioClip[] newClips)
    {
        if (newClips == null || newClips.Length == 0) return;

        // 1. FADE OUT & HAPUS SEMUA MUSIK LAMA
        // Kita salin list ke array sementara agar aman saat dimodifikasi di dalam loop
        AudioSource[] oldSources = activeSources.ToArray();
        activeSources.Clear(); // Kosongkan list utama untuk musik baru

        foreach (AudioSource source in oldSources)
        {
            if (source != null)
            {
                // Fade out volume ke 0
                source.DOFade(0f, crossFadeDuration).SetEase(Ease.Linear).OnComplete(() => 
                {
                    source.Stop();
                    Destroy(source); // Hancurkan komponen AudioSource setelah selesai
                });
            }
        }

        // 2. SETUP & FADE IN MUSIK BARU
        foreach (AudioClip clip in newClips)
        {
            if (clip != null)
            {
                // Buat AudioSource baru untuk setiap klip
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.clip = clip;
                newSource.loop = true;
                newSource.playOnAwake = false;
                newSource.volume = 0f; // Mulai dari 0

                // Mainkan & Fade In
                newSource.Play();
                newSource.DOFade(maxVolume, crossFadeDuration).SetEase(Ease.Linear);

                // Tambahkan ke daftar aktif
                activeSources.Add(newSource);
            }
        }
    }

    // Fungsi Fade Out untuk Transisi Dissolve (Mematikan semua musik aktif)
    public void FadeOutAllMusic(float duration)
    {
        foreach (AudioSource source in activeSources)
        {
            if (source != null)
            {
                source.DOFade(0f, duration).SetEase(Ease.Linear);
            }
        }
    }
}