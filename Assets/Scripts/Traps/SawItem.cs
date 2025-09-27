using UnityEngine;

public class SawItem : MonoBehaviour
{
	public float rotationSpeed = 20;
	void Update()
	{
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
	}

}
