using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class MainRpc : NetworkBehaviour
{
	public static MainRpc Instance { get; private set; }

	[SerializeField] private SyncedPrefabDb prefabs;
	
	private void Awake()
	{
		Instance = this;
	}

	public override void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
		base.OnDestroy();
	}

	[ServerRpc(RequireOwnership = false)]
	public void SpawnMe_ServerRpc(PlayerSpawnData data, ServerRpcParams serverRpcParams = default)
	{
		SpawnPlayer(serverRpcParams.Receive.SenderClientId, data);
	}

	private void SpawnPlayer(ulong id, PlayerSpawnData data)
	{
		NetworkUtil.Assert.IsHost();
		
		prefabs.GetActorPrefabs(data, out GameObject actorPrefab);
		
		var go = Instantiate(actorPrefab);
		
		// TODO hackerino
		var player = go.GetComponent<PlayerController>();
		var field = player.GetType().GetField("playerCamera", BindingFlags.Instance | BindingFlags.NonPublic);
		field.SetValue(player, FindFirstObjectByType<CameraController>());
		
		var no = go.GetComponent<NetworkObject>();
		no.SpawnWithOwnership(id);
	}
}

public struct PlayerSpawnData : INetworkSerializeByMemcpy
{
	public int rootPrefab;
}