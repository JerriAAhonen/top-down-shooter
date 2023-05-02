using System;
using UnityEngine;

public class AutomaticWeapon : Weapon
{
	[Tooltip("Rounds per minute")]
	[SerializeField] private float fireRate;

	private float timeBetweenShots => 60f / fireRate;

	private float elapsedSinceLastShot;

	public override void OnShoot(bool shootPressed, float dt, Action onShot)
	{
		float interval = timeBetweenShots;
		elapsedSinceLastShot += dt;

		if (currentlyReloading)
			return;

		if (shootPressed && elapsedSinceLastShot >= interval && roundsInMagazine > 0)
		{
			elapsedSinceLastShot = 0f;
			base.OnShoot(shootPressed, dt, onShot);
		}
	}
}
