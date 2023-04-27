using tds.Input;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private float rotationSpeed;
	[Header("Aiming and shooting")]
	[SerializeField] private LineRenderer aimLine;
	[SerializeField] private Transform shootPoint;
	[SerializeField] private ActorShooting shooting;
	[Header("Animations")]
	[SerializeField] private Animator animator;

	private readonly NetworkVariable<float> animatorX = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	private readonly NetworkVariable<float> animatorZ = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	private CameraController playerCamera;
	private Rigidbody rb;
	private Vector3 velocity;
	private Camera cam;
	private Plane mousePosPlane;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		cam = Camera.main;
		mousePosPlane = new Plane(Vector3.up, Vector3.zero);
		
		// TODO hackerino
		if (!playerCamera)
		{
			playerCamera = FindFirstObjectByType<CameraController>();
			playerCamera.SetPlayerTransform(transform);
		}
	}

	private void FixedUpdate()
	{
		if (!NetworkObject.IsOwner)
			return;
		
		PlayerMovement();
		PlayerRotation();
		shooting.Process();
		PlayerAnimations();
	}

	private void LateUpdate()
	{
		if (NetworkObject.IsOwner)
		{
			animatorX.Value = animator.GetFloat("X");
			animatorZ.Value = animator.GetFloat("Z");
		}
		else
		{
			animator.SetFloat("X", animatorX.Value);
			animator.SetFloat("Z", animatorZ.Value);
		}
	}

	private void PlayerMovement()
	{
		var movementInput = InputManager.Instance.MovementInput;
		var movement = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
		velocity = movementSpeed * Time.deltaTime * movement;
		rb.AddForce(velocity, ForceMode.VelocityChange);
	}

	private void PlayerRotation()
	{
		var ray = cam.ScreenPointToRay(InputManager.Instance.MousePosition);
		if (mousePosPlane.Raycast(ray, out var intersectionDist))
		{
			var point = ray.GetPoint(intersectionDist);
			var target = new Vector3(point.x, transform.position.y, point.z);

			// Since the shootpoint is offset to the side, get the rotation from the shootpoint to the traget
			var relativeRotationPos = shootPoint.position;
			relativeRotationPos.y = 0f;

			// TODO: If the aim pos is between the player and the shootpoint, this breaks
			var rotation = Quaternion.LookRotation(target - relativeRotationPos);
			rb.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

			playerCamera.SetMousePos(target);

			// Aiming line renderer
			var aimLineHeight = aimLine.GetPosition(0).y;
			var aimLineTarget = new Vector3(0f, aimLineHeight, Vector3.Distance(target, shootPoint.position));
			aimLine.SetPosition(1, aimLineTarget);
		}
	}

	private void PlayerAnimations()
	{
		var localVelocity = transform.InverseTransformDirection(velocity);
		localVelocity /= Time.deltaTime * movementSpeed;
		localVelocity = localVelocity.Clamp(-1f, 1f);

		animator.SetFloat("X", localVelocity.x);
		animator.SetFloat("Z", localVelocity.z);
	}
}
