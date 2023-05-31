using System;
using System.Collections;
using System.Net.Http;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkInitializer : MonoBehaviour
{
	[SerializeField] private BuildType type;
	[SerializeField] private string ip = "127.0.0.1";
	[SerializeField] private Optional<KeyCode> hostButton;
	[SerializeField] private SyncedPrefabDb prefabs;

	private bool hostButtonWasClicked;
	
	private IEnumerator Start()
	{
		while (NetworkManager.Singleton == null)
			yield return null;

		// Give one frame to check the button
		if (hostButton.HasValue)
			yield return null;

		var mgr = NetworkManager.Singleton;
		var ut = (UnityTransport) mgr.NetworkConfig.NetworkTransport;
		
		if (hostButton.HasValue && hostButtonWasClicked
			|| type == BuildType.Host
			|| type == BuildType.HostIfBuild && !Application.isEditor)
		{
			if (!mgr.StartHost())
			{
				Debug.LogError("Failed to start host.");
				yield break;
			}

			yield return null;
			yield return GetMyIp(result => ip = result);

			foreach (var p in prefabs.Systems)
			{
				var go = Instantiate(p);
				DontDestroyOnLoad(go);
				go.GetComponent<NetworkObject>().Spawn();
			}

			MainRpc.Instance.SpawnMe_ServerRpc(new PlayerSpawnData { prefab = 0 });
		}
		else
		{
			ut.ConnectionData.Address = ip;

			if (!mgr.StartClient())
			{
				Debug.LogError("Failed to start client.");
				yield break;
			}

			while (!mgr.IsConnectedClient)
				yield return new WaitForSeconds(0.1f);

			yield return null;
			MainRpc.Instance.SpawnMe_ServerRpc(new PlayerSpawnData { prefab = 0 });
		}

		enabled = false;
	}

	private void Update()
	{
		if (!hostButtonWasClicked && hostButton.TryGet(out KeyCode button))
			hostButtonWasClicked = Input.GetKey(button);
	}

	private IEnumerator GetMyIp(Action<string> onResult)
	{
		var httpClient = new HttpClient();
		var task = httpClient.GetStringAsync("https://api.ipify.org");
		while (!task.IsCompleted)
			yield return null;
		onResult(task.Result);
	}
}

public enum BuildType
{
	Client,
	Host,
	HostIfBuild
}
