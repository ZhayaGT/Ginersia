using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    // Instance statis ini memastikan hanya ada satu BGMManager di scene
    public static BGMManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        // --- Pola Singleton ---
        if (Instance == null)
        {
            // Jika belum ada instance, jadikan objek ini instance-nya
            Instance = this;
            
            // PENTING: Jangan hancurkan objek ini saat scene berganti
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Jika sudah ada instance lain, hancurkan objek yang baru ini
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Atur agar musik diputar berulang
        audioSource.loop = true;

        // Cek jika musik belum diputar (agar tidak mengganggu debugging)
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
    // Fungsi opsional untuk mengubah volume atau memutar musik lain
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    // Fungsi untuk memutar klip musik baru
    public void PlayNewMusic(AudioClip newClip)
    {
        if (audioSource.clip != newClip)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}