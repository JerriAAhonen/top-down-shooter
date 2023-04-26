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

	protected int roundsInMagazine;
	protected bool currentlyReloading;

	public Transform FrontGripRef => frontGripRef;
	public Transform RearGripRef => rearGripRef;

	private void Start()
	{
		roundsInMagazine = magazineCapacity;
	}

	public abstract void OnShoot(bool shootPressed, Action onShot);

	public virtual void OnReload()
	{
		StartCoroutine(DoReload());
	}

	protected virtual IEnumerator DoReload()
	{
		currentlyReloading = true;
		yield return new WaitForSeconds(reloadTime);
		currentlyReloading = false;

		roundsInMagazine = magazineCapacity;
	}
}
