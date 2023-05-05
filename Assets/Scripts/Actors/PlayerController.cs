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
	[SerializeField] private float animationTransitionSpeed;

	private CameraController playerCamera;
	private Rigidbody rb;
	private Vector3 velocity;
	private Vector3 animationVelocity;

	public Transform ShootPoint => shootPoint;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		
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

	public override void OnNetworkSpawn()
	{
		if (NetworkObject.IsOwner)
		{
			InputManager.Instance.Shoot += shooting.OnShootPressed;
			InputManager.Instance.Reload += shooting.OnReloadPressed;
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
		if (!CursorController.Instance.IsReady)
			return;

		var aimPoint = CursorController.Instance.AimPoint;
		var target = aimPoint.SetY(transform.position.y);

		// Since the shootpoint is offset to the side, get the rotation from the shootpoint to the traget
		var relativeRotationPos = shootPoint.position;
		relativeRotationPos.y = 0f;

		var rotation = Quaternion.LookRotation(target - relativeRotationPos);
		rb.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

		playerCamera.SetMousePos(target);

		// Aiming line renderer
		var aimLineTargetX = 0f;
		var aimLineTargetY = aimLine.GetPosition(0).y;
		var aimLineTargetZ = Vector3.Distance(target, shootPoint.position);

		var aimLineTarget = new Vector3(aimLineTargetX, aimLineTargetY, aimLineTargetZ);
		aimLine.SetPosition(1, aimLineTarget);
	}

	private void PlayerAnimations()
	{
		var localVelocity = transform.InverseTransformDirection(velocity);
		localVelocity /= Time.deltaTime * movementSpeed;
		localVelocity = localVelocity.Clamp(-1f, 1f);

		// Smooth out the transitions in animations
		animationVelocity = Vector3.Lerp(animationVelocity, localVelocity, Time.deltaTime * animationTransitionSpeed);

		animator.SetFloat("X", animationVelocity.x);
		animator.SetFloat("Z", animationVelocity.z);
	}
}
