using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleSystemProjectile : MonoBehaviour
{
	[SerializeField] private ParticleSystem impactPS;
	[SerializeField] private GameObject bulletHole;
	[SerializeField] private float bulletHoleDespawnDuration;
	[SerializeField] private LayerMask clutterMask;

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
			var ps = impactPSPool.Get();
			ps.transform.SetPositionAndRotation(
				e.intersection, 
				Quaternion.LookRotation(Vector3.Reflect(e.velocity, e.normal)));
			ps.Play();

			// Bullet holes
			var hole = bulletHolePool.Get();
			hole.transform.SetPositionAndRotation(
				e.intersection + e.normal * 0.1f,
				Quaternion.LookRotation(-e.normal));
			hole.transform.parent = other.transform;
		}

		if (other.layer == clutterMask)
		{
			var rb = other.GetComponent<Rigidbody>();
			rb.AddForce(events[0].velocity, ForceMode.Impulse);
		}
	}
}
