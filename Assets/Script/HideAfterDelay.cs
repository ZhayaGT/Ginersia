using UnityEngine;
using DG.Tweening; // WAJIB: Menggunakan DOTween

public class HideAfterDelay : MonoBehaviour
{
    [Header("Pengaturan Delay")]
    [Tooltip("Waktu tunggu (detik) sebelum objek disembunyikan.")]
    public float delay = 2.0f;

    [Header("Target")]
    [Tooltip("Jika dicentang, skrip ini akan menyembunyikan GameObject tempat ia dipasang.")]
    public bool hideSelf = true;

    [Tooltip("Daftar objek lain yang ingin disembunyikan (Opsional).")]
    public GameObject[] otherObjectsToHide;

    [Header("Trigger")]
    [Tooltip("Jika True, penghitungan mundur dimulai otomatis saat objek ini aktif (OnEnable).")]
    public bool autoStart = true;

    // Dipanggil setiap kali GameObject ini diaktifkan (SetActive(true))
    private void OnEnable()
    {
        if (autoStart)
        {
            Hide();
        }
    }

    // Fungsi publik yang bisa dipanggil dari script lain atau Button Event
    public void Hide()
    {
        // Menggunakan DOTween untuk delay
        // .SetLink(gameObject) memastikan timer berhenti jika object ini dihancurkan sebelum waktu habis
        DOVirtual.DelayedCall(delay, () => 
        {
            ExecuteHide();
        }).SetLink(gameObject);
    }

    private void ExecuteHide()
    {
        // 1. Sembunyikan diri sendiri
        if (hideSelf)
        {
            gameObject.SetActive(false);
        }

        // 2. Sembunyikan objek lain di array
        if (otherObjectsToHide != null)
        {
            foreach (GameObject obj in otherObjectsToHide)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
    
    // Fungsi pembantu untuk membatalkan delay (jika diperlukan)
    public void CancelHide()
    {
        DOTween.Kill(gameObject); // Mematikan semua tween yang terlink ke object ini
    }
}