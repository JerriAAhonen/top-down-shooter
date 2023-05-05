using System.Collections;
using tds.Input;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : Singleton<CursorController>
{
	[Header("Cursor")]
	[SerializeField] private Texture2D defaultCursor;
	[Header("Layers")]
	[SerializeField] private LayerMask clutterMask;
	[SerializeField] private LayerMask defaultAimHeightMask;
	[Header("Settings")]
	[SerializeField] private float minDistanceFromShootpoint;

	private Outline currentOutline;
	private PlayerController player;

	public bool IsReady { get; private set; }
	public Vector3 AimPoint { get; private set; }

	private IEnumerator Start()
	{
		Cursor.SetCursor(defaultCursor, new Vector2(256, 256), CursorMode.Auto);

		while (MainRpc.Instance == null || MainRpc.Instance.Players.Count == 0)
			yield return null;
		
		player = MainRpc.Instance.Players[0];
		IsReady = true;
	}

	private void Update()
	{
		if (!IsReady)
			return;

		var ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);
		if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clutterMask))
		{
			var outline = hit.collider.GetComponent<Outline>();
			if (outline != null && outline != currentOutline)
			{
				outline.enabled = true;
				currentOutline = outline;
			}

			var centerY = hit.collider.bounds.center.y;
			var hitPoint = hit.point.SetY(centerY);
			if (IsTooClose(hitPoint))
				return;

			AimPoint = hitPoint;
			return;
		}
		else if (currentOutline != null)
		{
			currentOutline.enabled = false;
			currentOutline = null;
		}
		else if (Physics.Raycast(ray, out hit, Mathf.Infinity, defaultAimHeightMask))
		{
			var hitPoint = hit.point.SetY(player.ShootPoint.position.y);
			if (IsTooClose(hitPoint))
				return;

			AimPoint = hitPoint;
			return;
		}
	}

	private bool IsTooClose(Vector3 hitPoint)
	{
		var from = player.transform.position.SetY(0f);
		var to = hitPoint.SetY(0f);

		return Vector3.Distance(from, to) < minDistanceFromShootpoint;
	}

	private void OnDrawGizmos()
	{
		var from = Camera.main.transform.position;
		var to = AimPoint - from;
		Gizmos.DrawRay(from, to);
	}
}
