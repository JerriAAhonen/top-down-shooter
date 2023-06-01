using Unity.Netcode;
using UnityEngine;

public class EnemyController : ActorController
{
	private AIState state;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		if (NetworkManager.Singleton.IsHost)
		{
			state = GetComponent<AIState>();
			if (state)
				state.OnEnter(null);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (NetworkManager.Singleton.IsHost && state)
		{
			state.OnUpdate(Time.fixedDeltaTime);

			foreach (var t in state.Transitions)
			{
				AIState target = t.MoveTo;
				if (target)
				{
					state.OnExit(target);
					target.OnEnter(state);
					state = target;
					break;
				}
			}
		}
	}
}
