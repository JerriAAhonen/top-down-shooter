using System;
using UnityEngine;

public class AutomaticWeapon : Weapon
{
	[Tooltip("Rounds per minute")]
	[SerializeField] private float fireRate;

	private float timeBetweenShots => 1f / fireRate / 60f;

	private float elapsedSinceLastShot;

	public override void OnShoot(bool shootPressed, Action onShot)
	{
		float interval = timeBetweenShots;
		elapsedSinceLastShot += Time.deltaTime;

		if (shootPressed && elapsedSinceLastShot >= interval)
		{
			// Preserve the overflow in order to keep a steady firerate
			elapsedSinceLastShot -= interval;

			if (roundsInMagazine > 0)
			{
				roundsInMagazine--;
				onShot?.Invoke();
			}
		}
	}
}
