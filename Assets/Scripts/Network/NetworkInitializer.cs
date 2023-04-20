using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkInitializer: MonoBehaviour
{
	[SerializeField] private BuildType type;

	private IEnumerator Start()
	{
		while (NetworkManager.Singleton == null)
			yield return null;

		var mgr = NetworkManager.Singleton;
		var ut = (UnityTransport) mgr.NetworkConfig.NetworkTransport;
		ut.ConnectionData.Address = "127.0.0.1";
		
		if (type == BuildType.Host || (type == BuildType.HostIfBuild && !Application.isEditor))
		{
			if (!mgr.StartHost())
			{
				Debug.LogError("Failed to start host.");
				yield break;
			}
			yield return new WaitForSeconds(2.0f);
			MainRpc.Instance.SpawnMe_ServerRpc(new PlayerSpawnData { actorPrefab = 0, visualsPrefab = 0 });
		}
		else
		{
			if (!mgr.StartClient())
			{
				Debug.LogError("Failed to start client.");
				yield break;
			}

			while (!mgr.IsConnectedClient)
				yield return new WaitForSeconds(0.1f);

			yield return new WaitForSeconds(2.0f);
			MainRpc.Instance.SpawnMe_ServerRpc(new PlayerSpawnData { actorPrefab = 0, visualsPrefab = 1 });
		}
	}
}

public enum BuildType
{
	Client,
	Host,
	HostIfBuild
}
