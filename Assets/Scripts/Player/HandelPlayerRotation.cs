using UnityEngine;

public class HandlePlayerRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 8f; // Dönme hızı
    [SerializeField] private bool randomRotationDirection = true; // Rastgele dönüş yönü
    
    private bool isRotating = false; // Dönme durumu
    private Quaternion targetRotation; // Hedef rotasyon (Quaternion)
    private bool isFacingRight = true; // Hangi yöne bakıyor

    void Start()
    {
        // Başlangıçta sağa bakıyor (0 derece)
        targetRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        // Smooth rotation güncelleme
        HandleSmoothRotation();
    }

    public void UpdateFacingDirection(float movementX)
    {
        // Rotation tabanlı smooth dönüş sistemi
        bool shouldFaceRight = movementX > 0;
        bool shouldFaceLeft = movementX < 0;
        
        if (shouldFaceRight && !isFacingRight)
        {
            // Sağa dön (Y rotasyonu 0 derece)
            isFacingRight = true;
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
            isRotating = true;
        }
        else if (shouldFaceLeft && isFacingRight)
        {
            // Sola dön (Y rotasyonu 180 derece)
            isFacingRight = false;
            targetRotation = Quaternion.Euler(0f, 180f, 0f);
            isRotating = true;
        }
    }

    private void HandleSmoothRotation()
    {
        if (!isRotating) return;
        
        // Quaternion.Slerp ile smooth rotasyon (360° wrap-around sorunu yok)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        // Dönüş tamamlandı mı kontrol et (Quaternion.Angle ile açı farkını ölç)
        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        if (angle < 1f) // 1 derece fark kaldığında dönüşü tamamla
        {
            transform.rotation = targetRotation;
            isRotating = false;
        }
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }

    // Manuel olarak yön ayarlamak için
    public void SetFacingDirection(bool facingRight)
    {
        if (facingRight && !isFacingRight)
        {
            isFacingRight = true;
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
            isRotating = true;
        }
        else if (!facingRight && isFacingRight)
        {
            isFacingRight = false;
            targetRotation = Quaternion.Euler(0f, 180f, 0f);
            isRotating = true;
        }
    }
}

