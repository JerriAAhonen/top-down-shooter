using Unity.Netcode;
using UnityEngine;

public class GlobalRpc : NetworkBehaviour
{
	private int playerCount;
	
	public static GlobalRpc Instance { get; private set; }
	
	private void Awake()
	{
		Instance = this;
	}

	[ServerRpc(RequireOwnership = false)]
	public void SpawnMeServerRpc(int visuals, ServerRpcParams serverRpcParams = default)
	{
		SpawnPlayer(serverRpcParams.Receive.SenderClientId, visuals);
	}

	private void SpawnPlayer(ulong id, int visuals)
	{
		var prefabs = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs;
		var playerPrefab = prefabs.Find(i => i.Prefab.name == "NetworkPlayer").Prefab;
		var visualsPrefab = prefabs.Find(i => i.Prefab.name == $"NetworkVisuals{visuals}").Prefab;
		
		var go = Instantiate(playerPrefab);
		var no = go.GetComponent<NetworkObject>();
		Spawn(no, id);
		
		var player = go.GetComponent<NetworkPlayer>();
		player.data.Value = new PlayerData { color = colors[playerCount++] };
		
		var dyn = Instantiate(visualsPrefab);
		var no2 = dyn.GetComponent<NetworkObject>();
		Spawn(no2, id);
		no2.TrySetParent(no, false);

		static void Spawn(NetworkObject networkObject, ulong id)
		{
			if (NetworkManager.Singleton.LocalClientId == id)
				networkObject.Spawn();
			else
				networkObject.SpawnWithOwnership(id);
		}
	}
	
	private Color[] colors = { Color.red, Color.cyan, Color.yellow };
}
