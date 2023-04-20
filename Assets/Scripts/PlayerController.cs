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
	[SerializeField] private Transform shootPoint;
	[SerializeField] private Projectile projectilePrefab;
	[SerializeField] private ParticleSystem projectilePS;
	[SerializeField] private ParticleSystem muzzleFlashPS;
	[Header("Animations")]
	[SerializeField] private Animator animator;

	private CharacterController cc;
	private Vector3 velocity;
	private Camera cam;
	private Plane mousePosPlane;

	private IObjectPool<Projectile> projectilePool;

	private void Awake()
	{
		cc = GetComponent<CharacterController>();
		cam = Camera.main;
		mousePosPlane = new Plane(Vector3.up, Vector3.zero);

		projectilePool = new ObjectPool<Projectile>(
			CreateProjectile,
			p =>
			{
				p.gameObject.SetActive(true);
			},
			p =>
			{
				p.gameObject.SetActive(false);
			},
			Destroy);

		Projectile CreateProjectile()
		{
			return Instantiate(projectilePrefab);
		}
	}

	private void Start()
	{
		InputManager.Instance.Shoot += PlayerShooting;
	}

	private void Update()
	{
		PlayerMovement();
		PlayerRotation();
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
			var aimLineTarget = new Vector3(0f, aimLineHeight, Vector3.Distance(target, transform.position));
			aimLine.SetPosition(1, aimLineTarget);
		}
	}

	private void PlayerShooting(bool shooting)
	{
		if (!shooting)
			return;

		// Raycast for htis
		var ray = new Ray(shootPoint.position, transform.forward);
		if (Physics.Raycast(ray, out var hit, 100f))
		{
			//Debug.Log($"Hit: {hit.collider.name}", hit.collider);
		}

		// Shoot physical projectile for visuals
		/*var projectile = projectilePool.Get();
		projectile.transform.position = shootPoint.position;
		projectile.transform.rotation = transform.rotation;
		projectile.Init(projectilePool.Release);*/

		projectilePS.Play();
		muzzleFlashPS.Play();
	}

	private void PlayerAnimations()
	{
		var localVelocity = transform.InverseTransformDirection(velocity);
		localVelocity /= Time.deltaTime;

		animator.SetFloat("MovementSpeedX", localVelocity.x);
		animator.SetFloat("MovementSpeedZ", localVelocity.z);
	}
}
