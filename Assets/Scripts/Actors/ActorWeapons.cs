using System;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;

public class ActorWeapons : MonoBehaviour
{
	[SerializeField] private List<WeaponMap> weaponMap;
	[SerializeField] private Transform gunRoot;

	private Dictionary<WeaponID, Weapon> weapons;
	private WeaponID currentWeaponID;
	private Weapon currentWeapon;

	public Weapon CurrentWeapon => currentWeapon;

	private void Awake()
	{
		weapons = new Dictionary<WeaponID, Weapon>();
		foreach (var item in weaponMap)
			weapons.Add(item.id, item.weapon);

		foreach (Transform child in gunRoot)
			child.gameObject.SetActive(false);
	}

	private void Start()
	{
		InputManager.Instance.SwitchWeapon += OnSwitchWeapon;
		ChangeWeapon(0);
	}

	private void OnSwitchWeapon(float f)
	{
		//Debug.Log($"Switch weapon, f: {f}");
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
		if (!weapons.ContainsKey(id))
			return;

		weapons[currentWeaponID].gameObject.SetActive(false);
		weapons[id].gameObject.SetActive(true);
		currentWeapon = weapons[id];
		currentWeaponID = id;

		currentWeapon.OnEquip();

		//Debug.Log($"New Weapon: {currentWeapon.DisplayName}");
	}

	[Serializable]
	public class WeaponMap
	{
		public WeaponID id;
		public Weapon weapon;
	}
}
