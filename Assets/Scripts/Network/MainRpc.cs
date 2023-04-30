using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class MainRpc : NetworkBehaviour
{
	public static MainRpc Instance { get; private set; }

	[SerializeField] private SyncedPrefabDb prefabs;

	public List<PlayerController> Players { get; private set; } = new();

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
		var go = Instantiate(prefabs.GetPlayer(data));
		var no = go.GetComponent<NetworkObject>();
		no.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);

		Players.Add(go.GetComponent<PlayerController>());
	}

	public void SpawnEnemy(EnemySpawner spawner)
	{
		NetworkUtil.Assert.IsHost();

		var data = new EnemySpawnData { prefab = 0 };
		var go = Instantiate(prefabs.GetEnemy(data), spawner.transform.position, spawner.transform.rotation);
		var no = go.GetComponent<NetworkObject>();
		no.Spawn();
	}
}

public struct PlayerSpawnData : INetworkSerializeByMemcpy
{
	public int prefab;
}

public struct EnemySpawnData : INetworkSerializeByMemcpy
{
	public int prefab;
}
