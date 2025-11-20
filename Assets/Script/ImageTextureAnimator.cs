using UnityEngine;
using UnityEngine.UI; // Diperlukan untuk komponen Image
using System.Collections;

[RequireComponent(typeof(Image))]
public class ImageTextureAnimator : MonoBehaviour
{
    [Tooltip("Drag semua Texture/Sprite yang membentuk animasi ke sini")]
    public Sprite[] animationFrames; 
    
    [Tooltip("Waktu tunda antar frame (dalam detik)")]
    public float frameDelay = 0.1f;

    private Image imageComponent;
    private int currentFrameIndex = 0;

    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (animationFrames.Length > 0)
        {
            // Pastikan gambar dimulai dari frame pertama
            imageComponent.sprite = animationFrames[0];
            
            // Mulai coroutine animasi
            StartCoroutine(AnimateFrames());
        }
        else
        {
            Debug.LogWarning("Array Animation Frames kosong!", this);
        }
    }

    private IEnumerator AnimateFrames()
    {
        while (true)
        {
            // Tunggu selama waktu tunda antar frame
            yield return new WaitForSeconds(frameDelay);

            // Pindah ke frame berikutnya
            currentFrameIndex++;
            if (currentFrameIndex >= animationFrames.Length)
            {
                currentFrameIndex = 0; // Kembali ke frame pertama
            }

            // Ganti sprite
            imageComponent.sprite = animationFrames[currentFrameIndex];
        }
    }
}