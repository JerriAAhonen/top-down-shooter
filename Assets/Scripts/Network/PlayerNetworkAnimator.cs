using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetworkAnimator : NetworkAnimator
{
	private void Update()
	{
		if (!NetworkObject.IsOwner || !NetworkObject.NetworkManager.IsClient)
			return;
		
		if (Input.GetKeyDown(KeyCode.A))
		{
			GetComponent<Animator>().SetFloat("Speed", 0.3f);
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			GetComponent<Animator>().SetFloat("Speed", 1);
		}
	}

	protected override bool OnIsServerAuthoritative()
	{
		return false;
	}
}
