using Unity.Netcode;
using UnityEngine;

public class ActorAnimatorSync : NetworkBehaviour
{
	[SerializeField] private Animator animator;

	private readonly NetworkVariable<float> animatorX = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	private readonly NetworkVariable<float> animatorZ = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
}
