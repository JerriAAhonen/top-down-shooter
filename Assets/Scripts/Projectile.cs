using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private TrailRenderer trail;

	private Action<Projectile> onRelease;
	private bool collideOnNextUpdate;
	private Vector3 collisionPosition;

	private void Update()
	{
		transform.position += movementSpeed * Time.deltaTime * transform.forward;

		if (collideOnNextUpdate)
		{
			transform.position = collisionPosition;

			Release();
			return;
		}

		var ray = new Ray(transform.position, transform.forward);
		if (Physics.Raycast(ray, out var hit, movementSpeed * Time.deltaTime + 0.1f))
		{
			collideOnNextUpdate = true;
			collisionPosition = hit.point;
		}

		Failsafe();
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log($"Hit: {collision.collider.name}", collision.collider);
		Release();
	}

	public void Init(Action<Projectile> onRelease)
	{
		this.onRelease += onRelease;
		collideOnNextUpdate = false;

		trail.Clear();

		failsafeElapsed = 0f;
	}

	private void Release()
	{
		onRelease?.Invoke(this);
		onRelease = null;
	}

	// Failsafe so that no projectile gets lost in the abyss
	private const float failsafeDuration = 20f;
	private float failsafeElapsed;
	private void Failsafe()
	{
		failsafeElapsed += Time.deltaTime;
		if (failsafeElapsed > failsafeDuration)
		{
			Debug.Log("Max time elapsed without hitting anything");
			Release();
		}
	}
}
