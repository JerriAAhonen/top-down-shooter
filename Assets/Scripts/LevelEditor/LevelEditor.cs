using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
	public enum Category { Wall, Door, Window }

	[SerializeField] private Vector2Int gridSize;
	[SerializeField] private float cellSize;
	[SerializeField] private Transform testCube;

	private Grid<int> grid;
	private Plane mouseInputPlane;
	private Transform buildGhost;
	private Vector3 previousRotation;

	private void Awake()
	{
		grid = new Grid<int>(gridSize, cellSize, new Vector3(-gridSize.x, 0, -gridSize.y) / 2f);
		mouseInputPlane = new Plane(Vector3.up, Vector3.zero);
	}

	private void Update()
	{
		if (buildGhost == null)
		{
			buildGhost = Instantiate(testCube);
			buildGhost.rotation = Quaternion.Euler(previousRotation);
		}

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (mouseInputPlane.Raycast(ray, out var distance))
		{
			var worldHitPoint = ray.GetPoint(distance);
			
			if (grid.GetGridAlignedPosition(worldHitPoint, out var gridAlignedWorldPos))
				buildGhost.position = gridAlignedWorldPos;
			else
				buildGhost.position = worldHitPoint;

			if (Input.GetMouseButtonDown(0))
				ChangeValue(worldHitPoint);
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			buildGhost.Rotate(0, 90, 0);
			previousRotation = buildGhost.rotation.eulerAngles;
		}
    }

	private void ChangeValue(Vector3 worldHitPoint)
	{
		if (grid.GetValue(worldHitPoint, out var prev))
		{
			grid.SetValue(worldHitPoint, prev + 1);
			buildGhost = null;
		}
	}
}
