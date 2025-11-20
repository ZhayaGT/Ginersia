using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LevelTransitioner : MonoBehaviour
{
    [Header("Pengaturan Rotasi Map")]
    [Tooltip("Transform Map (labirin) yang akan diputar")]
    public Transform mapTransform;
    [Tooltip("Rotasi Y awal map (misalnya 180 untuk level yang dimulai terbalik)")]
    public float initialYRotation = 180f;
    [Tooltip("Durasi animasi rotasi balik ke 0 derajat")]
    public float rotationDuration = 0.8f;
    public Ease rotationEase = Ease.InOutSine;

    [Header("Objek yang akan Ditampilkan Belakangan")]
    [Tooltip("Objek yang harus disembunyikan saat Scene dimuat dan akan dimunculkan setelah delay/rotasi.")]
    public GameObject[] objectsToReveal;
    [Tooltip("Delay (detik) sebelum objek dimunculkan kembali (setelah rotasi selesai).")]
    public float revealDelay = 0.2f;

    void Awake()
    {
        if (mapTransform == null)
        {
            mapTransform = this.transform;
            Debug.LogWarning("mapTransform tidak di-link, menggunakan transform GameObject ini.", this);
        }

        // 1. Sembunyikan semua objek yang ditandai untuk ditampilkan belakangan
        SetObjectsActive(objectsToReveal, false);

        // 2. Tentukan rotasi awal map (sesuai setting Inspector)
        Vector3 initialRotation = mapTransform.eulerAngles;
        initialRotation.y = initialYRotation;
        mapTransform.eulerAngles = initialRotation;
        
        Debug.Log($"Map diatur ke rotasi awal Y: {initialYRotation} derajat.");
    }

    void Start()
    {
        // Mulai transisi
        StartMapTransition();
    }

    private void StartMapTransition()
    {
        // Target rotasi adalah 0 derajat di sumbu Y
        Vector3 targetRotation = mapTransform.eulerAngles;
        targetRotation.y = 0f;

        Debug.Log($"Memulai rotasi balik map ke 0 derajat dalam {rotationDuration} detik.");

        // Animasi rotasi DOTween
        mapTransform.DORotate(targetRotation, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(rotationEase)
            .OnComplete(() => {
                // Setelah rotasi selesai, mulai coroutine untuk menampilkan objek
                StartCoroutine(RevealObjectsAfterDelay(revealDelay));
            });
    }

    private IEnumerator RevealObjectsAfterDelay(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        
        // Tampilkan semua objek yang harus muncul
        SetObjectsActive(objectsToReveal, true);
        Debug.Log("Objek telah dimunculkan kembali.");
    }

    // Fungsi pembantu untuk mengaktifkan/menonaktifkan array objek
    private void SetObjectsActive(GameObject[] objects, bool activeState)
    {
        if (objects == null) return;
        
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(activeState);
            }
        }
    }
}