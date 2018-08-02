using UnityEngine;
using System.Collections;

public class PlanetPlayerController : MonoBehaviour {

	public float cameraSensitivity = 1;
	public float normalMoveSpeed = 1;
	public float fastMoveFactor = 10;
	public float slowMoveFactor = 0.2f;

	private float rotationX = 0;
	private float rotationY = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

		

		transform.localRotation *= Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);


		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			if (Input.GetKey(KeyCode.E))
				transform.position += transform.up * fastMoveFactor * normalMoveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
				transform.position -= transform.up * fastMoveFactor * normalMoveSpeed * Time.deltaTime;
		} else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
			transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			if (Input.GetKey(KeyCode.E))
				transform.position += transform.up * slowMoveFactor * normalMoveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
				transform.position -= transform.up * slowMoveFactor * normalMoveSpeed * Time.deltaTime;
		} else {

			transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
			if (Input.GetKey(KeyCode.E))
				transform.position += transform.up * normalMoveSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Q))
				transform.position -= transform.up * normalMoveSpeed * Time.deltaTime;
		}
	}
}
