using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public abstract class Spawner : NetworkBehaviour
{
	public override void OnNetworkSpawn()
	{
		if (NetworkManager.Singleton.IsHost)
			StartCoroutine(Spawn());
	}

	protected abstract void OnReady();

	private IEnumerator Spawn()
	{
		while (MainRpc.Instance == null)
			yield return null;

		OnReady();
	}
}
