using UnityEngine;

public class GurzItem : MonoBehaviour
{
	public enum SwingAxis { X, Z }

	[Header("Profile A")]
	public bool profileA_FlipFacing = false; // if true, rotate initial y by 180
	[Tooltip("Minimum rotation speed (deg/s) for profile A")]
	public float profileA_MinSpeed = 60f;
	[Tooltip("Maximum rotation speed (deg/s) for profile A")]
	public float profileA_MaxSpeed = 180f;
	public SwingAxis profileA_Axis = SwingAxis.X;

	[Header("Profile B")]
	public bool profileB_FlipFacing = true;
	[Tooltip("Minimum rotation speed (deg/s) for profile B")]
	public float profileB_MinSpeed = 80f;
	[Tooltip("Maximum rotation speed (deg/s) for profile B")]
	public float profileB_MaxSpeed = 220f;
	public SwingAxis profileB_Axis = SwingAxis.Z;

	[Header("Profile Selection")]
	public bool randomizeProfile = true; // if true pick A or B at Start randomly
	public bool startWithProfileB = false; // used if randomizeProfile is false
	[Range(0f, 1f)] public float profileBChance = 0.5f; // chance to pick profile B when randomizing

	private float currentSpeed = 0f;
	private SwingAxis currentAxis = SwingAxis.X;

	void Start()
	{

	bool useB = randomizeProfile ? (Random.value < profileBChance) : startWithProfileB;

		if (useB)
		{
			currentSpeed = Random.Range(profileB_MinSpeed, profileB_MaxSpeed) * (Random.value < 0.5f ? 1f : -1f);
			currentAxis = profileB_Axis;
			if (profileB_FlipFacing) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		}
		else
		{
			currentSpeed = Random.Range(profileA_MinSpeed, profileA_MaxSpeed) * (Random.value < 0.5f ? 1f : -1f);
			currentAxis = profileA_Axis;
			if (profileA_FlipFacing) transform.rotation = Quaternion.Euler(0f, 0f, 180f);
		}
	}

	void Update()
	{
		// Rotate continuously on the selected axis
		if (currentAxis == SwingAxis.X)
			transform.Rotate(currentSpeed * Time.deltaTime, 0f, 0f, Space.Self);
		else
			transform.Rotate(0f, 0f, currentSpeed * Time.deltaTime, Space.Self);
	}
}



