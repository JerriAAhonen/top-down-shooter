using System.Collections.Generic;
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
	[SerializeField] private List<LineOfSightTarget> lineOfSightTargets;
	[SerializeField] private Transform eyePosition;
	[Header("Animations")]
	[SerializeField] protected Animator animator;
	[SerializeField] private float animationTransitionSpeed;

	protected Rigidbody rb;
	protected Vector3 animationVelocity;

	public IReadOnlyList<LineOfSightTarget> LineOfSightTargets => lineOfSightTargets;
	public Transform EyePosition => eyePosition;

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
		LineOfSightController.Instance.Register(this, this);
	}

	public override void OnDestroy()
	{
		LineOfSightController.Instance?.Unregister(this);
		base.OnDestroy();
	}

	public Transform ShootPoint => shootPoint;

	private void UpdateAnimations()
	{
		var localVelocity = transform.InverseTransformDirection(animationVelocity);
		localVelocity /= Time.deltaTime * movementSpeed;
		localVelocity = localVelocity.Clamp(-1f, 1f);

		// Smooth out the transitions in animations
		animationVelocity = Vector3.Lerp(animationVelocity, localVelocity, Time.deltaTime * animationTransitionSpeed);

		animator.SetFloat("X", localVelocity.x);
		animator.SetFloat("Z", localVelocity.z);
	}
}
