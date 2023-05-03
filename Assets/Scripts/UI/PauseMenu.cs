using System.Collections;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] private CanvasGroup cg;

	private void Start()
	{
		InputManager.Instance.Pause += OnPause;
		cg.SetVisible(false);
	}

	private void OnPause()
	{
		cg.Toggle();
	}

	private void OnDestroy()
	{
		InputManager.Instance.Pause -= OnPause;
	}
}
