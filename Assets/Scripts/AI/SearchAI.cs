using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class SearchAI : AIState
{
	[SerializeField] private float pathInterval = 1f;
	[SerializeField] private float speed = 3.5f;
	[SerializeField] private float angularSpeed = 120f;

	private List<AIRoomMarker> rooms;
	private List<AIRoomMarker> nonVisitedRooms;
	private NavMeshPath path;

	protected override IAITransition[] CreateTransitions()
	{
		return new[]
		{
			// TODO: Transition to CombatAI
			new LambdaAITransition(null, () => false)
		};
	}

	public override void OnInit()
	{
		PoolUtil.Get(ref rooms);
		PoolUtil.Get(ref nonVisitedRooms);
		path = new NavMeshPath();

		rooms.AddRange(FindObjectsByType<AIRoomMarker>(FindObjectsSortMode.None)); // TODO
	}

	private void OnDestroy()
	{
		PoolUtil.Release(ref rooms);
		PoolUtil.Release(ref nonVisitedRooms);
	}

	public override void OnEnter(AIState previous)
	{
		Agent.speed = speed;
		Agent.angularSpeed = angularSpeed;

		nonVisitedRooms.Clear();
		nonVisitedRooms.AddRange(rooms);
	}

	public override void OnUpdate(float dt)
	{
		if (Agent.velocity.sqrMagnitude > 0)
			return;

		Vector3 currentPos = transform.position;

		if (nonVisitedRooms.Count == 0)
			nonVisitedRooms.AddRange(rooms);

		(AIRoomMarker, Vector3, float) closest = default;
		foreach (var room in nonVisitedRooms)
		{
			var pos = room.GetRandomPosition();
			if (Agent.CalculatePath(pos, path))
			{
				float dist = CalculatePathLength(currentPos, pos);
				if (!closest.Item1 || dist < closest.Item3)
					closest = (room, pos, dist);
			}
		}

		if (!closest.Item1)
			return;
		if (!Agent.CalculatePath(closest.Item2, path))
			return;

		nonVisitedRooms.Remove(closest.Item1);
		Agent.SetPath(path);
	}

	public override void OnExit(AIState next)
	{
		Agent.SetPath(null);
	}

	private float CalculatePathLength(Vector3 start, Vector3 end)
	{
		corners ??= new Vector3[64];
		int count = path.GetCornersNonAlloc(corners);
		float totalDist = 0;
		Vector3 prev = start;

		for (int i = 0; i < count; i++)
		{
			totalDist += Vector3.Distance(prev, corners[i]);
			prev = corners[i];
		}

		totalDist += Vector3.Distance(prev, end);
		return totalDist;
	}

	private static Vector3[] corners;
	public static bool MaskContains(int mask, int value)
	{
		return ((value & mask) == value);
	}
}
