using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
	[SerializeField] private Vector2Int gridSize;
	[SerializeField] private float cellSize;
	[SerializeField] private bool enableGridDebug;
	[SerializeField] private LevelEditor_BuildGhost buildGhost;
	[SerializeField] private Transform placeableObjectsRoot;
	[SerializeField] private PlaceableObjectDatabase placeableObjectDb;

	private Grid<GridObject> grid;
	private Vector3 gridOrigin;
	private PlaceableObject currentPlaceableObject;
	private Dictionary<Vector3, Transform> placeableObjectInstances = new();
	
	private void Awake()
	{
		gridOrigin = new Vector3(-gridSize.x, 0, -gridSize.y) / 2f;
		grid = new Grid<GridObject>(gridSize, cellSize, gridOrigin, (Vector2Int gridPos, Vector3 worldPos, int _) => new GridObject(gridPos, worldPos));
		grid.EnableDebug(enableGridDebug);
		buildGhost.Init(this);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S)) { SaveLevel(); }
		if (Input.GetKeyDown(KeyCode.L)) { LoadLevel(); }
	}

	private void OnDrawGizmos()
	{
		if (!enableGridDebug) 
			return;

		grid?.OnDrawGizmos_DrawDebugData();
	}

	public bool OnTryPlaceObject(Vector3 worldPos, Vector3 rotation)
	{
		if (!grid.GetValue(worldPos, out var gridObject))
		{
			Debug.LogWarning("[LevelEditor] Position outside of grid");
			return false;
		}

		// Grid position already has an object on it
		if (gridObject.placeableObject != null)
		{
			Debug.LogWarning("[LevelEditor] Position already contains a placeable object");
			return false;
		}

		gridObject.placeableObject = currentPlaceableObject;
		gridObject.rotation = rotation;
		grid.SetValue(worldPos, gridObject);

		GetGridAlignedPosition(worldPos, out var gridAlignedWorldPos);
		InstantiateBuildPiece(gridAlignedWorldPos, gridObject);

		return true;
	}

	public void OnSelectPiece(PlaceableObject newPlaceableObject)
	{
		currentPlaceableObject = newPlaceableObject;
		buildGhost.SetGhost(newPlaceableObject.Prefab);
	}

	public bool GetGridAlignedPosition(Vector3 worldPos, out Vector3 result, bool suppressLogging = false)
	{
		return grid.GetGridAlignedPosition(worldPos, out result, suppressLogging);
	}

	private void InstantiateBuildPiece(Vector3 worldPos, GridObject gridObject)
	{
		var instance = Instantiate(gridObject.placeableObject.Prefab, placeableObjectsRoot).transform;
		instance.SetPositionAndRotation(worldPos, Quaternion.Euler(gridObject.rotation));
		Debug.Log($"placeableObjectInstances.Add({worldPos})");
		placeableObjectInstances.Add(worldPos, instance);
	}

	public void SaveLevel()
	{
		var saveObjects = new List<GridObjectSaveObject>(grid.Width * grid.Height);

		for (int x = 0; x < grid.Width; x++)
		{
			for(int y = 0; y < grid.Height; y++)
			{
				grid.GetValue(new Vector2Int(x, y), out var obj);
				var saveObj = new GridObjectSaveObject();
				saveObj.gridPosition = new SerializableVector2Int(obj.gridPos);
				saveObj.hasObject = obj.placeableObject != null;
				saveObj.id = placeableObjectDb.GetId(obj.placeableObject);
				saveObj.rotation = new SerializableVector3(obj.rotation);
				saveObjects.Add(saveObj);
			}
		}

		var gridSaveObject = new GridSaveObject();
		gridSaveObject.gridSize = new SerializableVector2Int(gridSize);
		gridSaveObject.origin = new SerializableVector3(gridOrigin);
		gridSaveObject.cellSize = cellSize;
		gridSaveObject.saveObjects = saveObjects;

		var saveData = JsonConvert.SerializeObject(gridSaveObject, Formatting.Indented);
		Debug.Log(saveData);

		var path = Application.dataPath + "/SavedLevels/Level01";
		File.WriteAllText(path, saveData);
	}

	public void LoadLevel()
	{
		// TODO:
		// Read all build pieces
		// Populate grid with pieces
		var path = Application.dataPath + "/SavedLevels/Level01";
		var json = File.ReadAllText(path);

		var gridSaveObject = JsonConvert.DeserializeObject<GridSaveObject>(json);

		grid?.DeleteDebugData();
		grid = new Grid<GridObject>(
			gridSaveObject.gridSize.ToVector2Int(),
			gridSaveObject.cellSize,
			gridSaveObject.origin.ToVector3(), 
			(Vector2Int gridPos, Vector3 worldPos, int index) =>
			{
				var gridObjectSaveObject = gridSaveObject.saveObjects.Find(x => x.gridPosition.ToVector2Int().Equals(gridPos));
				var gridObject = new GridObject(gridPos, worldPos);
				if (gridObjectSaveObject.hasObject)
				{
					gridObject.placeableObject = placeableObjectDb.GetObject(gridObjectSaveObject.id);
					gridObject.rotation = gridObjectSaveObject.rotation.ToVector3();
				}

				if (gridObject.placeableObject != null)
				{
					Debug.Log($"place object in: {gridPos}");
					grid.GetGridAlignedPosition(gridPos, out var gridAlignedWorldPos);
					InstantiateBuildPiece(gridAlignedWorldPos, gridObject);
				}

				return gridObject;
			});
	}
}

public class GridObject
{
	public GridObject(Vector2Int gridPos, Vector3 worldPos)
	{
		this.gridPos = gridPos;
		this.worldPos = worldPos;
	}

	public Vector2Int gridPos;
	public Vector3 worldPos;
	public Vector3 rotation;
	public PlaceableObject placeableObject;
}

[Serializable]
public class GridSaveObject
{
	public SerializableVector2Int gridSize;
	public SerializableVector3 origin;
	public float cellSize;
	public List<GridObjectSaveObject> saveObjects;
}

[Serializable]
public class GridObjectSaveObject
{
	public SerializableVector2Int gridPosition;
	public bool hasObject;
	public int id;
	public SerializableVector3 rotation;
}
