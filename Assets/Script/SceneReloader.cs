using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // WAJIB ada untuk New Input System

public class SceneReloader : MonoBehaviour
{
    [Header("Input Aksi")]
    [Tooltip("Link action 'Load' dari Input Action Asset Anda di sini.")]
    public InputActionReference loadAction;

    private void OnEnable()
    {
        if (loadAction != null)
        {
            loadAction.action.Enable();
            // Daftarkan fungsi yang akan dipanggil ketika aksi 'Load' dieksekusi
            loadAction.action.performed += OnLoadPerformed;
        }
    }

    private void OnDisable()
    {
        if (loadAction != null)
        {
            loadAction.action.performed -= OnLoadPerformed;
            loadAction.action.Disable();
        }
    }

    private void OnLoadPerformed(InputAction.CallbackContext context)
    {
        // 1. Dapatkan nama scene yang sedang aktif saat ini
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        Debug.Log($"Input 'Load' terdeteksi. Memuat ulang scene: {currentSceneName}");

        // 2. Muat ulang scene tersebut
        SceneManager.LoadScene(currentSceneName);
    }
}