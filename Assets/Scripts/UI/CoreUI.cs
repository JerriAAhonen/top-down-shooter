using System.Collections;
using System.Collections.Generic;
using tds.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoreUI : Singleton<CoreUI>
{
	[Header("Reload")]
	[SerializeField] private GameObject reloadMeterRoot;
	[SerializeField] private Image reloadMeterFill;
	[SerializeField] private Vector2 reloadMeterOffset;

	[Header("Ammunition")]
	[SerializeField] private TextMeshProUGUI ammoInMagazine;
	[SerializeField] private TextMeshProUGUI ammoTotal;

	protected override void Awake()
	{
		base.Awake();

		reloadMeterRoot.SetActive(false);
	}

	private void Update()
	{
		reloadMeterRoot.transform.position = InputManager.Instance.MousePosition + reloadMeterOffset;
	}

	public void ShowReload(bool reload, float dur)
	{
		if (!reload)
		{
			reloadMeterRoot.SetActive(false);
			return;
		}

		reloadMeterRoot.SetActive(true);
		ammoInMagazine.text = "-";
		LeanTween.value(0f, 1f, dur)
			.setOnUpdate(v => reloadMeterFill.fillAmount = v)
			.setOnComplete(() => reloadMeterRoot.SetActive(false));
	}

	public void UpdateAmmunition(int inMagazine, int total)
	{
		ammoInMagazine.text = inMagazine.ToString();
		ammoTotal.text = total.ToString();
	}
}
