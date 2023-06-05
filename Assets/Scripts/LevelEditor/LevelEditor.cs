using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
	[SerializeField] private Vector2Int gridSize;
	[SerializeField] private float cellSize;
	[SerializeField] private Transform testCube;

	private Grid grid;
	private Plane mouseInputPlane;

	private void Awake()
	{
		grid = new Grid(gridSize, cellSize, new Vector3(-gridSize.x, 0, -gridSize.y) / 2f);
		mouseInputPlane = new Plane(Vector3.up, Vector3.zero);
	}

	private void Update()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (mouseInputPlane.Raycast(ray, out var distance))
		{
			var worldHitPoint = ray.GetPoint(distance);
			if (Input.GetMouseButtonDown(0))
				ChangeValue(worldHitPoint);

			if (grid.GetGridAlignedPosition(worldHitPoint, out var gridAlignedWorldPos))
			{
				testCube.position = gridAlignedWorldPos;
			}
			else
				testCube.position = worldHitPoint;
		}
    }

	private void ChangeValue(Vector3 worldHitPoint)
	{
		if (grid.GetValue(worldHitPoint, out var prev))
			grid.SetValue(worldHitPoint, prev + 1);
	}
}
