using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponID
{
	AssaultRifle = 0,
	Shotgun = 1
}

/*
 * Update loop -> OnShoot(bool pressed)
 * 
 * 
 */

public abstract class Weapon : MonoBehaviour
{
	[SerializeField] private WeaponID weaponID;
	[SerializeField] private string displayName;
	[SerializeField] private Transform frontGripRef;
	[SerializeField] private Transform rearGripRef;

	[Header("Attributes")]
	[SerializeField] private int magazineCapacity;
	[SerializeField] private float reloadTime;

	protected int totalAmmo;
	protected int roundsInMagazine;
	protected bool currentlyReloading;

	public Transform FrontGripRef => frontGripRef;
	public Transform RearGripRef => rearGripRef;

	private void Start()
	{
		roundsInMagazine = magazineCapacity;
		totalAmmo = 200;
		CoreUI.Instance.UpdateAmmunition(roundsInMagazine, totalAmmo);
	}

	public virtual void OnShoot(bool shootPressed, float dt, Action onShot)
	{
		CoreUI.Instance.UpdateAmmunition(roundsInMagazine, totalAmmo);
	}

	public virtual void OnReload()
	{
		StartCoroutine(DoReload());
	}

	protected virtual IEnumerator DoReload()
	{
		CoreUI.Instance.ShowReload(true, reloadTime);

		currentlyReloading = true;
		yield return new WaitForSeconds(reloadTime);
		currentlyReloading = false;

		roundsInMagazine = magazineCapacity;
		totalAmmo -= roundsInMagazine;

		CoreUI.Instance.UpdateAmmunition(roundsInMagazine, totalAmmo);
	}
}
