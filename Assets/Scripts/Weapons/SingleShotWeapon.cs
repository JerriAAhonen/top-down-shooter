using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotWeapon : Weapon
{
	private bool hasReleasedShootButton;
	private bool hasCocked;

	protected override void Start()
	{
		base.Start();

		hasReleasedShootButton = true;
		hasCocked = true;
	}

	public override void OnShoot(bool shootPressed, float dt, Action onShot)
	{
		if (currentlyReloading)
			return;

		if (CanShoot())
		{
			base.OnShoot(shootPressed, dt, onShot);
			hasReleasedShootButton = false;
			hasCocked = false;
			//Debug.Log("Shoot");
		}
		else if (CanReleaseShootButton())
		{
			hasReleasedShootButton = true;
			//Debug.Log("Release shoot/cock button");
		}
		else if (CanCock())
		{
			hasCocked = true;
			hasReleasedShootButton = false;
			//Debug.Log("Cock weapon");
		}

		bool CanShoot() =>  shootPressed && hasReleasedShootButton && hasCocked && roundsInMagazine > 0;
		bool CanReleaseShootButton() => !shootPressed && !hasReleasedShootButton;
		bool CanCock() => shootPressed && hasReleasedShootButton && !hasCocked && roundsInMagazine > 0;
	}
}
