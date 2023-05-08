using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileHitDetector : MonoBehaviour
{
	private ParticleSystem ps;
	private int wallLayer;
	private Dictionary<GameObject, DestroyableWall> targets;

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();
		wallLayer = LayerMask.NameToLayer("Wall");
		PoolUtil.Get(ref targets);
	}

	private void OnDestroy()
	{
		PoolUtil.Release(ref targets);
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.layer != wallLayer)
			return;

		if (targets.TryGetValue(other, out var wall) && wall == null)
			return;

		wall = other.GetComponent<DestroyableWall>();
		targets[other] = wall;

		if (!wall)
			return;

		using (ListPool<ParticleCollisionEvent>.Get(out var events))
		{
			if (ps.GetCollisionEvents(other, events) == 0)
				return;

			using (ListPool<DestroyableWall.Hit>.Get(out var hits))
			{
				foreach (var e in events)
					hits.Add(new DestroyableWall.Hit { point = e.intersection, normal = e.normal });

				wall.ApplyHits(hits);
			}
		}
	}
}
