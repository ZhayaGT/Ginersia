using UnityEngine;
using DG.Tweening; // <-- WAJIB ADA

public class CameraShaker : MonoBehaviour
{
    // Variabel untuk menyimpan posisi asli kamera
    private Vector3 originalPosition;
    
    // Variabel untuk memastikan kita tidak menimpa shake
    private Tween currentShakeTween;

    void Start()
    {
        // Simpan posisi awal kamera
        originalPosition = transform.position;
    }

    // Fungsi ini akan dipanggil oleh skrip lain (BallAudio)
    public void TriggerShake(float strength, float duration)
    {
        // Jika sedang ada shake, hentikan dulu
        if (currentShakeTween != null && currentShakeTween.IsActive())
        {
            currentShakeTween.Kill();
            // Kembalikan ke posisi asli sebelum memulai shake baru
            transform.position = originalPosition;
        }

        // Mulai shake baru menggunakan DOTween
        // DOShakePosition akan menggetarkan posisi dalam radius 'strength'
        currentShakeTween = transform.DOShakePosition(
            duration,   // Durasi getaran
            strength,   // Kekuatan getaran
            10,         // Vibrato (seberapa cepat getarannya)
            90,         // Randomness
            false,      // fadeOut (kita atur true agar mulus)
            true        // fadeOut
        ).OnComplete(() => {
            // Pastikan kamera kembali ke posisi semula setelah selesai
            transform.position = originalPosition;
        });
    }
}