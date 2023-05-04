using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleSystemProjectile : MonoBehaviour
{
	[SerializeField] private ParticleSystem impactPS;
	[SerializeField] private GameObject bulletHole;
	[SerializeField] private LayerMask clutterMask;

	private ParticleSystem ps;
	private readonly List<ParticleCollisionEvent> events = new();

	private IObjectPool<ParticleSystem> impactPSPool;
	private readonly List<ParticleSystem> activeParticleSystems = new();

	// TODO: Pooling
	private IObjectPool<GameObject> bulletHolePool;
	private readonly List<GameObject> activeBulletHoles = new();

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

		// TODO: Pooling
		//bulletHolePool = new ObjectPool<GameObject>

		ParticleSystem CreateParticleSystem()
		{
			return Instantiate(impactPS);
		}
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
			// TODO: Pooling
			var hole = Instantiate(bulletHole);
			hole.transform.SetPositionAndRotation(
				e.intersection + e.normal * 0.1f,
				Quaternion.LookRotation(-e.normal));
		}

		if (other.layer == clutterMask)
		{
			var rb = other.GetComponent<Rigidbody>();
			rb.AddForce(events[0].velocity, ForceMode.Impulse);
		}
	}
}
