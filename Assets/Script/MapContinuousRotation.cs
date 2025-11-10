using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening; // <-- PASTIKAN INI ADA

[RequireComponent(typeof(Rigidbody2D))]
public class MapContinuousRotation : MonoBehaviour
{
    [Header("Pengaturan Rotasi")]
    public float rotationSpeed = 150f;

    [Header("Input Actions")]
    public InputActionReference leftAction;
    public InputActionReference rightAction;

    [Header("Koneksi Eksternal")]
    public LevelTimer levelTimer; 

    [Header("Pengaturan Selesai Level")]
    public float postResetDelay = 1.0f;
    public float flipDuration = 1.0f;

    // --- BARU: PENGATURAN "FEEL" ---
    [Tooltip("Ease untuk animasi reset ke 0. Coba 'Ease.OutBack' atau 'Ease.OutElastic' untuk efek 'snap'")]
    public Ease resetEase = Ease.OutBack; 
    [Tooltip("Ease untuk animasi membalik 180 derajat")]
    public Ease flipEase = Ease.OutQuad;
    // --- AKHIR BARU ---

    public GameObject[] objectsToHideOnFlip;
    public GameObject[] objectsToShowOnFlip;

    // --- State Machine ---
    private enum MapState { PlayerControl, Animating }
    private MapState currentState = MapState.PlayerControl;
    
    // --- Variabel Fisika ---
    private Rigidbody2D rb; 
    private float rotationInput; 

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb.bodyType != RigidbodyType2D.Kinematic)
        {
            Debug.LogWarning("Rigidbody di Peta Labirin HARUS Kinematic!", this.gameObject);
        }
    }

    // --- Proses State Machine ---
    void Update()
    {
        if (currentState == MapState.PlayerControl)
        {
            rotationInput = rightAction.action.ReadValue<float>() - leftAction.action.ReadValue<float>();
        }
    }

    // Terapkan rotasi fisika di FixedUpdate
    void FixedUpdate()
    {
        if (currentState == MapState.PlayerControl)
        {
            if (rotationInput != 0)
            {
                if (levelTimer != null)
                {
                    levelTimer.StartTimer();
                }

                float rotationAmount = -rotationInput * rotationSpeed * Time.fixedDeltaTime; 
                rb.MoveRotation(rb.rotation + rotationAmount); 
            }
        }
    }
    
    // --- FUNGSI RESET UTAMA (DIMODIFIKASI DENGAN ANIMASI BARU) ---
    public void StartResetRotation()
    {
        if (currentState != MapState.PlayerControl) return;

        currentState = MapState.Animating;
        leftAction.action.Disable();
        rightAction.action.Disable();

        // Buat Sekuens Animasi DOTween
        Sequence finishSequence = DOTween.Sequence();

        // --- Langkah 1: Reset rotasi Z ke 0 (selama 1 detik) ---
        // Menggunakan 'resetEase' (misal: OutBack) untuk efek "snap"
        finishSequence.Append(
            transform.DORotate(Vector3.zero, 1.0f)
                     .SetEase(resetEase) // <-- ANIMASI RESET BARU
        );

        // --- Langkah 2: Delay ---
        finishSequence.AppendInterval(postResetDelay);

        // --- Langkah 3: Sembunyikan objek ---
        finishSequence.AppendCallback(() => {
            Debug.Log("Menyembunyikan objek...");
            foreach (GameObject obj in objectsToHideOnFlip)
            {
                if (obj != null) obj.SetActive(false);
            }
        });

        // --- Langkah 4: Flip 180 Y + Putar 360 Z ("Corkscrew") ---
        // Ini adalah animasi "memutar spritenya"
        // Targetnya adalah Y=180 dan Z=360
        Vector3 targetRotation = new Vector3(0, 180, 360);
        finishSequence.Append(
            transform.DORotate(targetRotation, flipDuration, RotateMode.FastBeyond360)
                     .SetEase(flipEase) // <-- ANIMASI FLIP BARU
        );
        // 'RotateMode.FastBeyond360' memastikan ia berputar 360 derajat penuh
        // sambil juga berputar 180 di Y, alih-alih mengambil jalan pintas.

        // --- Langkah 5: Tampilkan objek hasil ---
        finishSequence.AppendCallback(() => {
            Debug.Log("Flip Selesai. Memunculkan objek hasil...");
            foreach (GameObject obj in objectsToShowOnFlip)
            {
                if (obj != null) obj.SetActive(true);
            }
        });
    }
}