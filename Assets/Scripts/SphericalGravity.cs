using UnityEngine;
using System.Collections;

public class SphericalGravity : MonoBehaviour {

	public float gravity = -9.8f;
	public Vector3 centerOfPlanet = Vector3.zero;
	private Rigidbody rigid;

	private void Awake()
	{
		rigid = transform.GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		Vector3 gravityUp = (transform.position - centerOfPlanet).normalized;
		Vector3 localUp = transform.up;

		// Apply downwards gravity to body
		rigid.AddForce(gravityUp * gravity);
		// Allign bodies up axis with the centre of planet
		transform.rotation = Quaternion.FromToRotation(localUp, gravityUp) * transform.rotation;
	}

	public void SetAsRegPlanet()
	{
		gravity = -gravity;
	}

	public void SetAsInenrPlanet()
	{
		//gravity = gravity;
	}
}