using UnityEngine;

public class LevelEditor_BuildGhost : MonoBehaviour
{
	private Transform ghost;
	private Vector3 ghostRotation = Vector3.zero;
	private Plane mouseInputPlane;
	private LevelEditor levelEditor;

	public void Init(LevelEditor levelEditor)
	{
		this.levelEditor = levelEditor;
		mouseInputPlane = new Plane(Vector3.up, Vector3.zero);
	}

	private void Update()
	{
		if (ghost == null)
			return;

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (mouseInputPlane.Raycast(ray, out var distance))
		{
			var worldHitPoint = ray.GetPoint(distance);

			// TODO: Smooth snap to grid instead of instant teleport to next grid pos
			if (levelEditor.GetGridAlignedPosition(worldHitPoint, out var gridAlignedWorldPos, true))
				ghost.position = gridAlignedWorldPos;
			else
				ghost.position = worldHitPoint;

			if (Input.GetMouseButtonDown(0))
			{
				var success = levelEditor.OnTryPlaceObject(worldHitPoint, ghostRotation);
				Debug.Log(success ? "Piece built!" : "Can't build here");
			}
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			Rotate();
		}
	}

	public void SetGhost(GameObject ghostPrefab)
	{
		Debug.Log($"Instantiate ghost");

		if (ghost != null)
			Destroy(ghost.gameObject);

		ghost = Instantiate(ghostPrefab).transform;
		ghost.rotation = Quaternion.Euler(ghostRotation);
	}

	public void ClearGhost()
	{
		if (ghost != null)
			Destroy(ghost.gameObject);
		ghost = null;
	}

	private void Rotate()
	{
		ghost.Rotate(0, 90, 0);
		ghostRotation = ghost.rotation.eulerAngles;
	}
}
