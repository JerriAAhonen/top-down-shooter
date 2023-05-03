using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponID
{
	AssaultRifle = 0,
	Shotgun = 1
}

public abstract class Weapon : MonoBehaviour
{
	[SerializeField] private WeaponID weaponID;
	[SerializeField] private string displayName;
	[SerializeField] private Transform frontGripRef;
	[SerializeField] private Transform rearGripRef;

	[Header("Attributes")]
	[SerializeField] private int magazineCapacity;
	[SerializeField] private float reloadTime;
	[SerializeField] private WeaponOutput weaponOutput;

	protected int totalAmmo;
	protected int roundsInMagazine;
	protected bool currentlyReloading;

	public WeaponOutput WeaponOutput => weaponOutput;
	public Transform FrontGripRef => frontGripRef;
	public Transform RearGripRef => rearGripRef;
	public string DisplayName => displayName;

	protected virtual void Start()
	{
		roundsInMagazine = magazineCapacity;
		totalAmmo = 200;
		CoreUI.Instance.UpdateAmmunition(roundsInMagazine, totalAmmo);
	}

	/// <summary>
	/// Call base.OnShoot() when the shot was executed
	/// </summary>
	/// <param name="shootPressed"></param>
	/// <param name="dt"></param>
	/// <param name="onShot">Did shoot</param>
	public virtual void OnShoot(bool shootPressed, float dt, Action onShot)
	{
		CoreUI.Instance.UpdateAmmunition(roundsInMagazine, totalAmmo);
		roundsInMagazine--;
		onShot?.Invoke();
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
