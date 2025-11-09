using UnityEngine;
using UnityEngine.InputSystem; // WAJIB ada

public class MapContinuousRotation : MonoBehaviour
{
    [Header("Pengaturan Rotasi")]
    public float rotationSpeed = 150f;

    [Header("Input Actions")]
    public InputActionReference leftAction;
    public InputActionReference rightAction;

    // --- BARU ---
    // Variabel state untuk tahu jika kita sedang me-reset rotasi
    private bool isResetting = false;

    // --- Manajemen Input ---
    private void OnEnable()
    {
        leftAction.action.Enable();
        rightAction.action.Enable();
    }

    private void OnDisable()
    {
        leftAction.action.Disable();
        rightAction.action.Disable();
    }

    // --- Proses Rotasi ---
    void Update()
    {
        // --- BARU ---
        // Cek apakah kita sedang dalam mode reset
        if (isResetting)
        {
            // --- Logika RESET ---
            // Target kita adalah rotasi 0, 0, 0 (Quaternion.identity)
            Quaternion targetIdentity = Quaternion.identity;

            // Putar secara bertahap kembali ke 0 menggunakan kecepatan yang sama
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,     // Rotasi saat ini
                targetIdentity,         // Target (0,0,0)
                50 * Time.deltaTime // Kecepatan
            );
        }
        else
        {
            // --- Logika INPUT PEMAIN (Skrip lama Anda) ---
            
            // 1. Baca nilai input
            float rotationInput = rightAction.action.ReadValue<float>() - leftAction.action.ReadValue<float>();

            // 3. Jika ada input (bukan 0)
            if (rotationInput != 0)
            {
                // 4. Terapkan rotasi
                float rotationAmount = -rotationInput * rotationSpeed * Time.deltaTime;
                transform.Rotate(0, 0, rotationAmount);
            }
        }
    }

    // --- BARU ---
    // Ini adalah fungsi publik yang akan dipanggil oleh FinishZone
    public void StartResetRotation()
    {
        isResetting = true;
        
        // Matikan input action agar pemain tidak bisa mengganggu reset
        leftAction.action.Disable();
        rightAction.action.Disable();
    }
}