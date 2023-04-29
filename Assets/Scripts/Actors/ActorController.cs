using Unity.Netcode;
using UnityEngine;

public abstract class ActorController : NetworkBehaviour
{
	[SerializeField] protected float movementSpeed;
	[SerializeField] protected float rotationSpeed;
	[Header("Aiming and shooting")]
	[SerializeField] protected LineRenderer aimLine;
	[SerializeField] protected Transform shootPoint;
	[SerializeField] protected ActorShooting shooting;
	[Header("Animations")]
	[SerializeField] protected Animator animator;

	protected Rigidbody rb;
	protected Vector3 velocity;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	protected virtual void FixedUpdate()
	{
		if (!NetworkObject.IsOwner)
			return;

		UpdateAnimations();
	}

	public override void OnNetworkSpawn()
	{
		//LineOfSightController.Instance.Register(this);
	}

	public override void OnDestroy()
	{
		//LineOfSightController.Instance?.Unregister(this);
		base.OnDestroy();
	}

	public Transform ShootPoint => shootPoint;

	private void UpdateAnimations()
	{
		var localVelocity = transform.InverseTransformDirection(velocity);
		localVelocity /= Time.deltaTime * movementSpeed;
		localVelocity = localVelocity.Clamp(-1f, 1f);

		animator.SetFloat("X", localVelocity.x);
		animator.SetFloat("Z", localVelocity.z);
	}
}
