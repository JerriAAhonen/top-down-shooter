using tds.Input;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private float rotationSpeed;
	[Header("Aim")]
	[SerializeField] private LineRenderer aimLine;
	[SerializeField] private CameraController playerCamera;
	[Header("Shooting")]
	[SerializeField] private Weapon currentWeapon;
	[SerializeField] private float shootingInterval;
	[SerializeField] private Transform shootPoint;
	[SerializeField] private ParticleSystem projectilePS;
	[SerializeField] private ParticleSystem casingPS;
	[SerializeField] private ParticleSystem muzzleFlashPS;
	[SerializeField] private ParticleSystem muzzleSmokePS;
	[Header("Animations")]
	[SerializeField] private Animator animator;

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

	public override void OnNetworkSpawn()
	{
		if (NetworkObject.IsOwner)
		{
			InputManager.Instance.Shoot += OnShootPressed;
			InputManager.Instance.Reload += OnReloadPressed;
		}
	}

	private void FixedUpdate()
	{
		if (!NetworkObject.IsOwner)
			return;
		
		PlayerMovement();
		PlayerRotation();
		PlayerShooting();
		PlayerAnimations();
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

	private bool shootPressed;

	private void OnShootPressed(bool shootPressed)
	{
		this.shootPressed = shootPressed;
	}

	private void OnReloadPressed()
	{
		currentWeapon?.OnReload();
	}

	private void PlayerShooting()
	{
		currentWeapon?.OnShoot(shootPressed, OnShot);

		void OnShot()
		{
			Vector3 from = shootPoint.position;
			Vector3 direction = transform.forward;

			// Only execute the shot here for clients, since host's execution happens later.
			if (!NetworkObject.IsOwnedByServer)
				ExecuteShot(from, direction);

			Shoot_ServerRpc(from, direction);
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

	private void ExecuteShot(Vector3 from, Vector3 direction)
	{
		projectilePS.Play();
		casingPS.Play();
		muzzleFlashPS.Play();
		muzzleSmokePS.Play();
	}

	#region RPC

	[ServerRpc(RequireOwnership = true)]
	private void Shoot_ServerRpc(Vector3 from, Vector3 direction, ServerRpcParams serverRpcParams = default)
	{
		ExecuteShot(from, direction);
		Shoot_ClientRpc(from, direction);

		var ray = new Ray(from, direction);
		if (Physics.Raycast(ray, out var hit, 100f))
		{
			// TODO: Deal damage
		}
	}

	[ClientRpc]
	private void Shoot_ClientRpc(Vector3 from, Vector3 direction)
	{
		// Execute shot only if not owned (owner's execution happens instantly when shooting)
		if (!NetworkObject.IsOwner)
			ExecuteShot(from, direction);
	}

	#endregion
}
