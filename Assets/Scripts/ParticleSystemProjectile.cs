using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class ParticleSystemProjectile : MonoBehaviour
{
	[SerializeField] private ParticleSystem impactPS;
	[SerializeField] private GameObject bulletHole;
	[SerializeField] private float bulletHoleDespawnDuration;
	[SerializeField] private LayerMask clutterMask;
	[SerializeField] private LayerMask actorMask;

	private ParticleSystem ps;
	private readonly List<ParticleCollisionEvent> events = new();

	private IObjectPool<ParticleSystem> impactPSPool;
	private readonly List<ParticleSystem> activeParticleSystems = new();

	private IObjectPool<GameObject> bulletHolePool;
	private readonly Dictionary<GameObject, float> activeBulletHoles = new();

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();

		impactPSPool = new ObjectPool<ParticleSystem>(
			CreateParticleSystem,
			ps =>
			{
				ps.gameObject.SetActive(true);
				activeParticleSystems.Add(ps);
			},
			ps =>
			{
				ps.gameObject.SetActive(false);
				activeParticleSystems.Remove(ps);
			},
			Destroy);

		bulletHolePool = new ObjectPool<GameObject>(
			CreateBulletHole,
			bh =>
			{
				bh.SetActive(true);
				activeBulletHoles.Add(bh, Time.realtimeSinceStartup);
			},
			bh =>
			{
				bh.SetActive(false);
				bh.transform.parent = null;
				activeBulletHoles.Remove(bh);
			},
			Destroy);

		ParticleSystem CreateParticleSystem() => Instantiate(impactPS);
		GameObject CreateBulletHole() => Instantiate(bulletHole);
	}

	private void LateUpdate()
	{
		foreach (var ps in activeParticleSystems)
		{
			if (!ps.IsAlive())
			{
				impactPSPool.Release(ps);
				return;
			}
		}

        foreach (var bh in activeBulletHoles)
        {
			if (Time.realtimeSinceStartup - bh.Value > bulletHoleDespawnDuration)
            {
				bulletHolePool.Release(bh.Key);
				return;
            }
        }
	}

	private void OnParticleCollision(GameObject other)
	{
		ps.GetCollisionEvents(other, events);
		foreach (var e in events)
		{
			// Impact Effect
			var ps = impactPSPool.Get();
			ps.transform.SetPositionAndRotation(
				e.intersection, 
				Quaternion.LookRotation(Vector3.Reflect(e.velocity, e.normal)));
			ps.Play();

			// Bullet Holes
			if (BitMaskUtil.MaskDoesNotContainLayer(actorMask, other.layer))
			{
				var hole = bulletHolePool.Get();
				hole.transform.SetPositionAndRotation(
					e.intersection + e.normal * 0.1f,
					Quaternion.LookRotation(-e.normal));
				hole.transform.parent = other.transform;
			}

			// Clutter Damage/Explosions
			if (BitMaskUtil.MaskContainsLayer(clutterMask, other.layer))
			{
				var clutter = other.GetComponent<Clutter>();
				clutter.RigidBody.AddForce(events[0].velocity * 0.2f, ForceMode.Impulse);
				if (clutter.Damage(5))
					clutter.Break(20f, events[0].intersection, 2f);
			}
		}
	}
}
