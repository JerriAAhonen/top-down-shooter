using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleSystemProjectile : MonoBehaviour
{
	[SerializeField] private ParticleSystem impactPS;

	private ParticleSystem ps;
	private readonly List<ParticleCollisionEvent> events = new();

	// Impact Particle System pool
	private IObjectPool<ParticleSystem> pool;
	private readonly List<ParticleSystem> activeParticleSystems = new();

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();

		pool = new ObjectPool<ParticleSystem>(
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
				pool.Release(ps);
				return;
			}
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		ps.GetCollisionEvents(other, events);
		foreach (var e in events)
		{
			var ps = pool.Get();
			ps.transform.SetPositionAndRotation(
				e.intersection, 
				Quaternion.LookRotation(Vector3.Reflect(e.velocity, e.normal)));
			ps.Play();
		}
	}
}
