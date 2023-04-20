using System;
using System.Collections;
using System.Collections.Generic;
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
		}
		else
		{
			if (!mgr.StartClient())
			{
				Debug.LogError("Failed to start client.");
				yield break;
			}
		}

		yield return new WaitForSeconds(2.0f);
		GlobalRpc.Instance.SpawnMeServerRpc(0);
	}
}

public enum BuildType
{
	Client,
	Host,
	HostIfBuild
}
