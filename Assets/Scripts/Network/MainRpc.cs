using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainRpc : NetworkBehaviour
{
	public static MainRpc Instance { get; private set; }

	[SerializeField] private PrefabDb prefabs;
	
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
		
		prefabs.GetActorPrefabs(data, out GameObject actorPrefab, out GameObject visualsPrefab);
		
		var go = Instantiate(actorPrefab);
		var no = go.GetComponent<NetworkObject>();
		no.SpawnWithOwnership(id);
		
		var player = go.GetComponent<NetworkPlayer>();
		Color[] colors = { Color.red, Color.cyan, Color.yellow };
		int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
		player.data.Value = new PlayerData { color = colors[playerCount - 1] };
		
		var dyn = Instantiate(visualsPrefab);
		var no2 = dyn.GetComponent<NetworkObject>();
		no2.SpawnWithOwnership(id);
		no2.TrySetParent(no, false);
	}
}

public struct PlayerSpawnData : INetworkSerializeByMemcpy
{
	public int rootPrefab;
	public int visualPrefab;
}