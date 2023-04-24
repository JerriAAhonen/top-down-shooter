using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
	private readonly NetworkVariable<int> equippedWeapon = new();
	private readonly NetworkVariable<float> animatorX = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	private readonly NetworkVariable<float> animatorZ = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	private Animator animator;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
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

	public override void OnNetworkSpawn()
	{
		if (NetworkObject.IsOwner)
			return;
		
		equippedWeapon.OnValueChanged += EquipWeapon;
		EquipWeapon(equippedWeapon.Value, equippedWeapon.Value);
	}

	public override void OnDestroy()
	{
		equippedWeapon.OnValueChanged -= EquipWeapon;
		base.OnDestroy();
	}

	private void EquipWeapon(int oldWeapon, int newWeapon)
	{
		// TODO
	}
}
