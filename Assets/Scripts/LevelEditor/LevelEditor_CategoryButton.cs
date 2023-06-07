using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor_CategoryButton : MonoBehaviour
{
	[SerializeField] private LevelEditor.Category category;

	public event Action<LevelEditor.Category> OnClick;

	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(OnClicked);
	}

	private void OnClicked()
	{
		OnClick?.Invoke(category);
	}

	/*[CustomEditor(typeof(LevelEditor_CategoryButton))]
	public class LevelEditor_CategoryButton_CustomInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var category = (LevelEditor_CategoryButton)target;
			category.category = (LevelEditor.Category)EditorGUILayout.EnumPopup("Category", LevelEditor.Category.Wall);

			base.OnInspectorGUI();
		}
	}*/
}
