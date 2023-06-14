using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUI : MonoBehaviour
{
	[SerializeField] private LevelEditor levelEditor;
	[Space]
	[SerializeField] private List<LevelEditor_CategoryButton> categoryButtons;
	[Space]
	[SerializeField] private Transform placeableObjectsRoot;
	[SerializeField] private LevelEditor_PlaceableObjectButton placeableObjectButtonPrefab; 
	[Space]
	[SerializeField] private PlaceableObjectDatabase placeableObjectsDb;

	private void Awake()
	{
		foreach (var button in categoryButtons)
		{
			button.OnClick += OnCategoryClicked;
		}
	}

	private void OnCategoryClicked(PlaceableObjectCategory category)
	{
		Debug.Log($"Category clicked: {category}");

		// TODO: Pooling

		foreach(Transform child in placeableObjectsRoot)
			Destroy(child.gameObject);

		foreach (var piece in placeableObjectsDb.GetObjects(category))
		{
			var pieceButton = Instantiate(placeableObjectButtonPrefab, placeableObjectsRoot);
			pieceButton.Init(piece);
			pieceButton.OnClick += OnPieceClicked;
		}
	}

	private void OnPieceClicked(PlaceableObject piece)
	{
		Debug.Log($"Piece clicked: {piece}");
		levelEditor.OnSelectPiece(piece);
	}
}
