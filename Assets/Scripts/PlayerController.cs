using System.Collections;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private float rotationSpeed;
	[Header("Aim")]
	[SerializeField] private LineRenderer aimLine;
	[SerializeField] private CameraController playerCamera;
	[Header("Shooting")]
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
	}

	private void Start()
	{
		InputManager.Instance.Shoot += OnShoot;
	}

	private void Update()
	{
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
			var rotation = Quaternion.LookRotation(target - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

			playerCamera.SetMousePos(target);

			// Aiming line renderer
			var aimLineHeight = aimLine.GetPosition(0).y;
			var aimLineTarget = new Vector3(0f, aimLineHeight, Vector3.Distance(target, shootPoint.position));
			aimLine.SetPosition(1, aimLineTarget);
		}
	}

	private float timeSinceLastShot;
	private bool currentlyShooting;

	private void OnShoot(bool shooting)
	{
		currentlyShooting = shooting;
	}

	private void PlayerShooting()
	{
		timeSinceLastShot += Time.deltaTime;

		if (!currentlyShooting)
			return;

		if (timeSinceLastShot > shootingInterval)
		{
			Shoot();
			timeSinceLastShot = 0f;
		}
		
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
