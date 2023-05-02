using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotWeapon : Weapon
{
	private bool hasReleasedShootButton;

	protected override void Start()
	{
		base.Start();

		hasReleasedShootButton = true;
	}

	public override void OnShoot(bool shootPressed, float dt, Action onShot)
	{
		if (currentlyReloading)
			return;

		if (shootPressed && hasReleasedShootButton && roundsInMagazine > 0)
		{
			base.OnShoot(shootPressed, dt, onShot);
			hasReleasedShootButton = false;
			Debug.Log("Shoot");
		}
		else if (!shootPressed)
		{
			hasReleasedShootButton = true;
			Debug.Log("Release shoot button");
		}
		else if (hasReleasedShootButton && shootPressed && roundsInMagazine == 0)
		{
			OnReload();
			Debug.Log("Reload");
		}
	}
}
