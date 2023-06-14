using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grid<TGridObject>
{
	private readonly Vector2Int size;
	private readonly Vector3 origin;
	private readonly float cellSize;
	private readonly TGridObject[,] data;
	private readonly Vector3 cellCenterOffset;
	private TextMeshPro[,] debugData;

	public int Width => size.x;
	public int Height => size.y;

	public event EventHandler GridValueChanged;
	public class GridValueChangedEventArgs : EventArgs
	{
		public Vector2Int gridPos;
		public TGridObject value;
	}

	public Grid(Vector2Int size, float cellSize, Vector3 origin, Func<Vector2Int, Vector3, int, TGridObject> createGridObject)
	{
		this.size = size;
		this.origin = origin;
		this.cellSize = cellSize;
		cellCenterOffset = new Vector3(cellSize, 0, cellSize) / 2f;
		data = new TGridObject[size.x, size.y];

		int index = 0;
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				var gridPos = new Vector2Int(x, y);
				GetWorldPosition(gridPos, out var worldPos);
				data[x, y] = createGridObject(gridPos, worldPos, index);
				index++;
			}
		}

		Debug.Log($"Created a new grid({size})");
		
		CreateDebugData();
	}

	private void CreateDebugData()
	{
		debugData = new TextMeshPro[size.x, size.y];

		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				GameObject go = new("GridDebugText", typeof(TextMeshPro));

				GetWorldPosition(new Vector2Int(x, y), out var pos);
				go.transform.position = pos + cellCenterOffset;
				go.transform.rotation = Quaternion.Euler(90, 0, 0);

				var textMesh = go.GetComponent<TextMeshPro>();
				textMesh.text = $"{x},{y}";
				textMesh.alignment = TextAlignmentOptions.Center;
				textMesh.enableAutoSizing = true;
				textMesh.fontSizeMin = 1f;
				textMesh.fontSizeMax = 3.45f;
				debugData[x, y] = textMesh;
			}
		}
	}

	public void DeleteDebugData()
	{
		if (debugData == null) 
			return;

		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				GameObject.Destroy(debugData[x, y].gameObject);
			}
		}
	}

	public void OnDrawGizmos_DrawDebugData()
	{
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				GetWorldPosition(new Vector2Int(x, y), out var corner1);
				GetWorldPosition(new Vector2Int(x, y + 1), out var corner2);
				GetWorldPosition(new Vector2Int(x + 1, y), out var corner3);

				Gizmos.DrawLine(corner1, corner2);
				Gizmos.DrawLine(corner1, corner3);
			}
		}

		GetWorldPosition(new Vector2Int(0, size.y), out var topLeftCorner);
		GetWorldPosition(new Vector2Int(size.x, size.y), out var topRightCorner);
		GetWorldPosition(new Vector2Int(size.x, 0), out var bottomRightCorner);
		Gizmos.DrawLine(topLeftCorner, topRightCorner);
		Gizmos.DrawLine(topRightCorner, bottomRightCorner);
	}

	#region Getters

	/// <summary>
	/// Get grid cell value
	/// </summary>
	/// <param name="gridPos">Int position in grid coordinates</param>
	/// <param name="result"></param>
	/// <returns>True if grid pos in inside grid</returns>
	public bool GetValue(Vector2Int gridPos, out TGridObject result)
	{
		result = default;

		if (!IsInside(gridPos))
		{
			Debug.LogWarning("[Grid] Position outside of grid");
			return false;
		}

		result = data[gridPos.x, gridPos.y];
		return true;
	}

	/// <summary>
	/// Get grid cell value
	/// </summary>
	/// <param name="worldPos"></param>
	/// <param name="result"></param>
	/// <returns>True if world pos is inside grid</returns>
	public bool GetValue(Vector3 worldPos, out TGridObject result)
	{
		result = default;

		if (!GetGridPosition(worldPos, out var gridPos))
			return false;

		result = data[gridPos.x, gridPos.y];
		return true;
	}

	/// <summary>
	/// Get a grid coordinates corresponding to the worldPosition
	/// </summary>
	/// <param name="worldPosition"></param>
	/// <param name="result"></param>
	/// <returns>True if worldPosition is inside grid</returns>
	private bool GetGridPosition(Vector3 worldPosition, out Vector2Int result, bool suppressLogging = false)
	{
		result = new Vector2Int();

		var x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
		var y = Mathf.FloorToInt((worldPosition - origin).z / cellSize); // Grid y = World z
		var gridPos = new Vector2Int(x, y);

		if (!IsInside(gridPos))
		{
			if (!suppressLogging) Debug.LogWarning("[Grid] Position outside of grid");
			return false;
		}

		result = gridPos;
		return true;
	}

	private bool GetWorldPosition(Vector2Int gridPos, out Vector3 result)
	{
		result = new Vector3(gridPos.x, 0, gridPos.y) * cellSize + origin; // Grid y = World z
		return IsInside(gridPos);
	}

	/// <summary>
	/// Snap worldPos to the center of the nearest grid cell
	/// </summary>
	/// <param name="worldPos"></param>
	/// <param name="result"></param>
	/// <returns>True if worldPos is inside grid</returns>
	public bool GetGridAlignedPosition(Vector3 worldPos, out Vector3 result, bool suppressLogging = false)
	{
		result = new Vector3();

		if (!GetGridPosition(worldPos, out var gridPos, suppressLogging))
			return false;

		GetWorldPosition(gridPos, out result);
		result += cellCenterOffset;
		return true;
	}

	public bool GetGridAlignedPosition(Vector2Int gridPos, out Vector3 result)
	{
		result = new Vector3();

		if (!IsInside(gridPos))
			return false;

		GetWorldPosition(gridPos, out var worldPos);
		result = worldPos + cellCenterOffset;
		return true;
	}

	#endregion

	#region Setters

	/// <summary>
	/// Set the corresponding grid cell data to value
	/// </summary>
	/// <param name="worldPos"></param>
	/// <param name="value"></param>
	/// <returns>True if worldPos is inside grid</returns>
	public bool SetValue(Vector3 worldPos, TGridObject value)
	{
		if (!GetGridPosition(worldPos, out var gridPos))
			return false;

		SetValue(gridPos, value);
		return true;
	}

	private void SetValue(Vector2Int gridPos, TGridObject value)
	{
		if (!IsInside(gridPos))
		{
			Debug.LogWarning("[Grid] Position outside of grid");
			return;
		}

		data[gridPos.x, gridPos.y] = value;

		GridValueChanged?.Invoke(this, new GridValueChangedEventArgs { gridPos = gridPos, value = value });

		if (debugData == null)
			return;

		debugData[gridPos.x, gridPos.y].text = data[gridPos.x, gridPos.y].ToString();
	}

	#endregion

	public bool IsInside(Vector2Int gridPos)
	{
		return gridPos.x >= 0 && gridPos.x < size.x && gridPos.y >= 0 && gridPos.y < size.y;
	}

	public void EnableDebug(bool enable)
	{
		foreach (var cell in debugData)
			cell.enabled = enable;
	}
}
