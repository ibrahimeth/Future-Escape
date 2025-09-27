using UnityEngine;

public class GurzItem : MonoBehaviour
{
	public float rotationSpeed = 180f; // Derece/sn -in
    private float angle = 0f;
	void Update()
	{
		transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
	}
}
