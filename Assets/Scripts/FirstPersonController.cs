using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {

	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float runModifier = 3;
	public float stalk = 0.5f;
	public float jumpForce = 220;
	public float gravity = -9.8f;
	public LayerMask groundedMask;

	// System vars
	bool grounded;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float verticalLookRotation;
	Transform cameraTransform;
	Rigidbody rigid;

	Transform planet;

	void Awake()
	{
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		cameraTransform = Camera.main.transform;

		rigid = GetComponent<Rigidbody>();
		planet = GameObject.FindGameObjectWithTag("Planet").transform;

		// Disable rigid gravity and rotation as this is simulated in GravityAttractor script
		rigid.useGravity = false;
		rigid.constraints = RigidbodyConstraints.FreezeRotation;
	}

	void Update()
	{

		// Look rotation:
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

		// Calculate movement:
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");

		Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		float moveSpeedModifier = 1;
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			moveSpeedModifier *= runModifier;
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			moveSpeedModifier *= stalk;
		targetMoveAmount *= moveSpeedModifier;
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

		// Jump
		if (Input.GetButtonDown("Jump")) {
			if (grounded) {
				rigid.AddForce(transform.up * jumpForce);
			}
		}

		// Grounded check
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) {
			grounded = true;
		} else {
			grounded = false;
		}

	}

	void FixedUpdate()
	{
		// Apply movement to rigid
		Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		rigid.MovePosition(rigid.position + localMove);

		Vector3 gravityUp = (planet.position - transform.position).normalized;
		Vector3 localUp = transform.up;

		// Apply downwards gravity to body
		rigid.AddForce(gravityUp * gravity);
		// Allign bodies up axis with the centre of planet
		transform.rotation = Quaternion.FromToRotation(localUp, gravityUp) * transform.rotation;
	}
}
