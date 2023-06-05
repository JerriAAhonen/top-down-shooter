using TMPro;
using UnityEngine;

public class Grid
{
	private readonly Vector2Int size;
	private readonly Vector3 origin;
	private readonly float cellSize;
	private readonly int[,] data;
	private readonly TextMeshPro[,] debugData;
	private readonly Vector3 cellCenterOffset;

	public Grid(Vector2Int size, float cellSize, Vector3 origin)
	{
		this.size = size;
		this.origin = origin;
		this.cellSize = cellSize;
		data = new int[size.x, size.y];
		debugData = new TextMeshPro[size.x, size.y];
		cellCenterOffset = new Vector3(cellSize, 0, cellSize) / 2f;

		Debug.Log($"Created a new grid({size})");

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
				textMesh.fontSize = 3.45f;
				debugData[x, y] = textMesh;

				GetWorldPosition(new Vector2Int(x, y), out var corner1);
				GetWorldPosition(new Vector2Int(x, y + 1), out var corner2);
				GetWorldPosition(new Vector2Int(x + 1, y), out var corner3);

				Debug.DrawLine(corner1, corner2, Color.white, 100f);
				Debug.DrawLine(corner1, corner3, Color.white, 100f);
			}
		}

		GetWorldPosition(new Vector2Int(0, size.y), out var topLeftCorner);
		GetWorldPosition(new Vector2Int(size.x, size.y), out var topRightCorner);
		GetWorldPosition(new Vector2Int(size.x, 0), out var bottomRightCorner);
		Debug.DrawLine(topLeftCorner, topRightCorner, Color.white, 100f);
		Debug.DrawLine(topRightCorner, bottomRightCorner, Color.white, 100f);
	}

	#region Getters

	/// <summary>
	/// Get grid cell value
	/// </summary>
	/// <param name="worldPos"></param>
	/// <param name="result"></param>
	/// <returns>True if world pos is inside grid</returns>
	public bool GetValue(Vector3 worldPos, out int result)
	{
		result = 0;

		if (!GetGridPosition(worldPos, out var gridPos))
			return false;

		result = data[gridPos.x, gridPos.y];
		return true;
	}

	private bool GetGridPosition(Vector3 worldPosition, out Vector2Int result)
	{
		result = new Vector2Int();

		var x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
		var y = Mathf.FloorToInt((worldPosition - origin).z / cellSize); // Grid y = World z
		var gridPos = new Vector2Int(x, y);

		if (!IsInside(gridPos))
			return false;

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
	public bool GetGridAlignedPosition(Vector3 worldPos, out Vector3 result)
	{
		result = new Vector3();

		if (!GetGridPosition(worldPos, out var gridPos))
			return false;

		GetWorldPosition(gridPos, out result);
		result += cellCenterOffset;
		return true;
	}

	#endregion

	#region Setters

	public bool SetValue(Vector3 worldPos, int value)
	{
		if (!GetGridPosition(worldPos, out var gridPos))
			return false;

		SetValue(gridPos, value);
		return true;
	}

	private void SetValue(Vector2Int gridPos, int value)
	{
		if (!IsInside(gridPos))
			return;

		data[gridPos.x, gridPos.y] = value;
		UpdateCell(gridPos);
	}

	#endregion

	private void UpdateCell(Vector2Int gridPos)
	{
		if (!IsInside(gridPos))
			return;

		debugData[gridPos.x, gridPos.y].text = data[gridPos.x, gridPos.y].ToString();
	}

	public bool IsInside(Vector2Int gridPos)
	{
		return gridPos.x >= 0 && gridPos.x < size.x && gridPos.y >= 0 && gridPos.y < size.y;
	}
}
