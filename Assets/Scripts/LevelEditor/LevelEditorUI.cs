using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUI : MonoBehaviour
{
	[SerializeField] private List<LevelEditor_CategoryButton> categoryButtons;

	private void Awake()
	{
		foreach (var button in categoryButtons)
		{
			button.OnClick += OnCategoryClicked;
		}
	}

	private void OnCategoryClicked(LevelEditor.Category category)
	{
		Debug.Log($"Category clicked: {category}");
	}
}
