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

	private CharacterController cc;
	private Vector3 velocity;
	private Camera cam;
	private Plane mousePosPlane;

	private void Awake()
	{
		cc = GetComponent<CharacterController>();
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

	private void Update()
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
		cc.Move(velocity);
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

			var rotation = Quaternion.LookRotation(target - relativeRotationPos);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

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
		currentWeapon.OnReload();
	}

	private void PlayerShooting()
	{
		currentWeapon.OnShoot(shootPressed, Shoot);

		void Shoot()
		{
			// Raycast for htis
			var ray = new Ray(shootPoint.position, transform.forward);
			if (Physics.Raycast(ray, out var hit, 100f))
			{
				//Debug.Log($"Hit: {hit.collider.name}", hit.collider);
			}

			projectilePS.Play();
			casingPS.Play();
			muzzleFlashPS.Play();
			muzzleSmokePS.Play();
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
