using Unity.Netcode;
using UnityEngine.Pool;

public class EnemyController : ActorController
{
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (NetworkObject.IsOwner)
		{
			using (ListPool<NetworkBehaviour>.Get(out var visible))
			{
				//LineOfSightController.Instance.GetVisible(this, visible);
				// TODO aim for the closest target
			}
		}
	}
}
