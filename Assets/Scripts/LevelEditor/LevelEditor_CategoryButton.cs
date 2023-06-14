using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor_CategoryButton : MonoBehaviour
{
	[SerializeField] private PlaceableObjectCategory category;

	public event Action<PlaceableObjectCategory> OnClick;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(OnClicked);
	}

	private void OnClicked()
	{
		OnClick?.Invoke(category);
	}
}
