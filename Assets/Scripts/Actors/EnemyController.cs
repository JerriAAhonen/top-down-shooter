using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : ActorController
{
	private NavMeshAgent agent;
	private NavMeshPath path;
	private PlayerController target;
	private float pathValidFor;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		if (NetworkObject.IsOwner)
		{
			agent = GetComponent<NavMeshAgent>();
			path = new NavMeshPath();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (NetworkManager.Singleton.IsHost)
		{
			MoveToPlayer();
		}
	}

	private void MoveToPlayer()
	{
		// TODO: Target priorization
		// TODO: Different states (exploration, combat, etc.)

		if (pathValidFor > 0)
			pathValidFor -= Time.fixedDeltaTime;
		if (pathValidFor > 0)
			return;

		target = MainRpc.Instance.Players.FirstOrDefault();
		if (!target)
			return;

		var targetPos = target.transform.position;
		if (Vector3.Distance(transform.position, targetPos) < 1 || !agent.CalculatePath(targetPos, path))
			return;

		agent.SetPath(path);
		pathValidFor = 1.5f;
	}
}
