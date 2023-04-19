using System.Collections;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private float rotationSpeed;
	[Header("Aim")]
	[SerializeField] private LineRenderer aimLine;
	[SerializeField] private CameraController playerCamera;
	[Header("Animations")]
	[SerializeField] private Animator animator;

	private CharacterController cc;
	private Vector3 velocity;
	private Camera cam;
	private Plane mousePosPlane;

	// Debug
	

	private void Awake()
	{
		cc = GetComponent<CharacterController>();
		cam = Camera.main;
		mousePosPlane = new Plane(Vector3.up, Vector3.zero);
	}

	private void Update()
	{
		PlayerMovement();
		PlayerRotation();
		UpdateAnimations();
	}

	private void PlayerMovement()
	{
		var movementInput = InputManager.Instance.MovementInput;
		var movement = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
		velocity = movementSpeed * Time.deltaTime * movement;
		cc.Move(velocity);
	}

	private void PlayerRotation()
	{
		var ray = cam.ScreenPointToRay(InputManager.Instance.MousePosition);
		if (mousePosPlane.Raycast(ray, out var intersectionDist))
		{
			var point = ray.GetPoint(intersectionDist);
			var target = new Vector3(point.x, transform.position.y, point.z);
			var rotation = Quaternion.LookRotation(target - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

			playerCamera.SetMousePos(target);

			// Aiming line renderer
			var aimLineTarget = new Vector3(0f, 0.1f, Vector3.Distance(target, transform.position));
			aimLine.SetPosition(1, aimLineTarget);
		}
	}

	private void UpdateAnimations()
	{
		var localVelocity = transform.InverseTransformDirection(velocity);
		localVelocity /= Time.deltaTime;

		animator.SetFloat("MovementSpeedX", localVelocity.x);
		animator.SetFloat("MovementSpeedZ", localVelocity.z);
	}
}
