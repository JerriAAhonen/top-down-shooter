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

		if (shootPressed && elapsedSinceLastShot >= interval && roundsInMagazine > 0)
		{
			roundsInMagazine--;
			elapsedSinceLastShot = 0f;
			onShot?.Invoke();
			base.OnShoot(shootPressed, dt, onShot);
		}
	}
}
