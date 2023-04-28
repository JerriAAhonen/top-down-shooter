using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class PlayerNetworkTransform : NetworkTransform
{
	protected override bool OnIsServerAuthoritative()
	{
		return false;
	}
}