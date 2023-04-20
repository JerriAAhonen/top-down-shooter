using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemProjectile : MonoBehaviour
{
	[SerializeField] private ParticleSystem impactPS;

	private ParticleSystem ps;
	private List<ParticleCollisionEvent> events = new();

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void OnParticleCollision(GameObject other)
	{
		ps.GetCollisionEvents(other, events);
		foreach (var e in events)
		{
			Instantiate(impactPS, e.intersection, Quaternion.LookRotation(Vector3.Reflect(e.velocity, e.normal)));
		}
	}
}
