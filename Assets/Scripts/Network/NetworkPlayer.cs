using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;
	[HideInInspector]
	public NetworkVariable<PlayerData> data = new();

	private void Awake()
	{
		data.OnValueChanged += UpdateVisuals;
	}

	public override void OnNetworkSpawn()
	{
		UpdateVisuals(data.Value, data.Value);
	}

	public override void OnDestroy()
	{
		data.OnValueChanged -= UpdateVisuals;
		base.OnDestroy();
	}

	private void UpdateVisuals(PlayerData oldData, PlayerData newData)
	{
		Material mat = meshRenderer.material;
		mat.color = newData.color;
	}
}

public struct PlayerData : INetworkSerializeByMemcpy
{
	public Color color;
}