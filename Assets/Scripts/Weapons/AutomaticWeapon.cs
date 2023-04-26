using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticWeapon : Weapon
{
	[Tooltip("Rounds per minute")]
	[SerializeField] private float fireRate;

	private float timeBetweenShots => 1f / fireRate / 60f;

	private float elapsedSinceLastShot;

	public override void OnShoot(bool shootPressed, Action onShot)
	{
		if (elapsedSinceLastShot < timeBetweenShots)
		{
			elapsedSinceLastShot += Time.deltaTime;
			return;
		}

		if (roundsInMagazine <= 0)
		{
			// Play empty magazine audio clip
			return;
		}

		if (!shootPressed)
		{
			return;
		}

		roundsInMagazine--;
		if (roundsInMagazine <= 0)
		{
			Debug.LogWarning("Empty magazine");
		}

		elapsedSinceLastShot = 0f;
		onShot?.Invoke();
	}
}
