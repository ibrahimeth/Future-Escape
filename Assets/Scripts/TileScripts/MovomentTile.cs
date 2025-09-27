using System;
using UnityEditor.MPE;
using UnityEngine;

public class movomentTile : MonoBehaviour
{
    [SerializeField] public float speed = 5f;        // Başlangıç hızı
    [SerializeField] private float speedIncrease = 0.1f; // Zamanla hız artışı
    [SerializeField] private float destroyXOffset = -10f;   // Yok olma pozisyonu (x)
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private Rigidbody rb;
    private float tileWidth;

    void Start()
    {
        speed = FutureEscape.Tiles.GlobalTileSettings.speed;
        speedIncrease = FutureEscape.Tiles.GlobalTileSettings.speedIncrease;
        Renderer rend = GetComponentInChildren<Renderer>();
        tileWidth = rend.bounds.size.x;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        Move();
        CheckDestroy();
        IncreaseGlobalSpeed();
    }

    private void Move()
    {
        rb.MovePosition(transform.position + Vector3.left * FutureEscape.Tiles.GlobalTileSettings.speed * Time.deltaTime);
    }

    private void CheckDestroy()
    {
        if (transform.position.x + tileWidth / 2 < -tileWidth + destroyXOffset)
        {
            Destroy(gameObject);
        }

    }

    private void IncreaseGlobalSpeed()
    {
        FutureEscape.Tiles.GlobalTileSettings.speed += FutureEscape.Tiles.GlobalTileSettings.speedIncrease * Time.deltaTime;
        if (FutureEscape.Tiles.GlobalTileSettings.speed > maxSpeed)
            FutureEscape.Tiles.GlobalTileSettings.speed = maxSpeed;
        else
        {
            //Debug.Log("Current Speed: " + FutureEscape.Tiles.GlobalTileSettings.speed);
        }
    }
}
