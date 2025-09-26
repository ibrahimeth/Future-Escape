using UnityEngine;

public class movomentTile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;        // Başlangıç hızı
    [SerializeField] private float speedIncrease = 0.1f; // Zamanla hız artışı
    [SerializeField] private float destroyX = -10f;   // Yok olma pozisyonu (x)
    [SerializeField] private float maxSpeed = 20f;

   void Update()
    {
        Move();
        CheckDestroy();
        IncreaseSpeed();
    }

    private void Move()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    private void CheckDestroy()
    {
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }

    private void IncreaseSpeed()
    {
        speed = Mathf.Min(speed + speedIncrease * Time.deltaTime, maxSpeed);
    }
}
