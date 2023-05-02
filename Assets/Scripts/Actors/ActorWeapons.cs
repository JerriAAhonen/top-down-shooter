using System;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;

public class ActorWeapons : MonoBehaviour
{
	[SerializeField] private List<WeaponMap> weaponMap;
	[SerializeField] private Transform gunRoot;

	private WeaponID currentWeaponID;
	private Weapon currentWeapon;

	public Weapon CurrentWeapon => currentWeapon;

	private void Start()
	{
		ChangeWeapon(0);
		InputManager.Instance.SwitchWeapon += OnSwitchWeapon;
	}

	private void OnSwitchWeapon(float f)
	{
		Debug.Log($"Switch weapon, f: {f}");
		if (f > 0)
			NextWeapon();
		else if (f < 0) 
			PreviousWeapon();
	}

	public void NextWeapon()
	{
		var currentId = (int)currentWeaponID;
		currentId++;
		var newId = Mathf.Repeat(currentId, weaponMap.Count);
		ChangeWeapon((WeaponID)newId);
	}

	public void PreviousWeapon()
	{
		var currentId = (int)currentWeaponID;
		currentId--;
		var newId = Mathf.Repeat(currentId, weaponMap.Count);
		ChangeWeapon((WeaponID)newId);
	}

	public void ChangeWeapon(WeaponID id)
	{
		if (currentWeapon != null)
			currentWeapon.gameObject.SetActive(false);

		var newMap = weaponMap.Find(x => x.id == id);
		newMap.weapon.gameObject.SetActive(true);
		currentWeaponID = id;
		currentWeapon = newMap.weapon;
	}

	[Serializable]
	public class WeaponMap
	{
		public WeaponID id;
		public Weapon weapon;
	}
}
