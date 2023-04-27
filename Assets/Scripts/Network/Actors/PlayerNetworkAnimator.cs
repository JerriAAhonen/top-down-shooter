using System.Reflection;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetworkAnimator : NetworkAnimator, ISerializationCallbackReceiver
{
	protected override bool OnIsServerAuthoritative()
	{
		return false;
	}
	
#if UNITY_EDITOR
	public new void OnBeforeSerialize()
	{
		if (this && !Animator)
		{
			var a = GetComponent<Animator>();
			if (a)
			{
				var field = GetType().BaseType.GetField("m_Animator", BindingFlags.Instance | BindingFlags.NonPublic);
				field.SetValue(this, a);
			}
		}

		base.OnBeforeSerialize();
	}
#endif
}
