using UnityEngine;

public class KeepWorldRotation : MonoBehaviour
{
    // Rotasi target yang ingin dipertahankan dalam koordinat dunia.
    // Biasanya Quaternion.identity (tidak ada rotasi), tapi bisa diubah.
    public Quaternion targetWorldRotation = Quaternion.identity; 

    // Untuk memastikan skrip ini berjalan setelah rotasi parent di FixedUpdate
    void LateUpdate() 
    {
        // Ambil rotasi parent saat ini
        Quaternion parentRotation = transform.parent.rotation;

        // Hitung rotasi lokal yang dibutuhkan untuk menghasilkan targetWorldRotation
        // Rumusnya: Local Rotation = Inverse(Parent Rotation) * Target World Rotation
        transform.localRotation = Quaternion.Inverse(parentRotation) * targetWorldRotation;
    }
}