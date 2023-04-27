using Unity.Netcode;
using UnityEngine;

public class ActorHealth : NetworkBehaviour
{
	private readonly NetworkVariable<int> health = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	public override void OnNetworkSpawn()
	{
		health.OnValueChanged += OnHealthChanged;
	}

	public override void OnDestroy()
	{
		health.OnValueChanged -= OnHealthChanged;
		base.OnDestroy();
	}

	public void DealDamage(int amount)
	{
		if (health.Value > 0)
			health.Value = Mathf.Max(health.Value - amount, 0);
	}

	private void OnHealthChanged(int oldValue, int newValue)
	{
		// TODO Visualize damage

		if (newValue <= 0 && NetworkManager.Singleton.IsHost)
		{
			// TODO Kill
			NetworkObject.Despawn(true);
		}
	}
}
