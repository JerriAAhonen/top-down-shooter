using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor_PlaceableObjectButton : MonoBehaviour
{
	private PlaceableObject piece;

	public event Action<PlaceableObject> OnClick;

	public void Init(PlaceableObject piece)
	{
		this.piece = piece;

		GetComponent<Image>().sprite = piece.Icon;
		GetComponent<Button>().onClick.AddListener(OnClicked);
	}

	private void OnClicked()
	{
		OnClick?.Invoke(piece);
	}
}
