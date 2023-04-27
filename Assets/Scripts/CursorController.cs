using System.Collections;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
	[Header("Cursor")]
	[SerializeField] private Texture2D defaultCursor;
	[SerializeField] private LayerMask clutterMask;

	private Outline currentOutline;

	private void Start()
	{
		Cursor.SetCursor(defaultCursor, new Vector2(256, 256), CursorMode.Auto);
	}

	private void Update()
	{
		var ray = Camera.main.ScreenPointToRay(InputManager.Instance.MousePosition);
		if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clutterMask))
		{
			var outline = hit.collider.GetComponent<Outline>();
			if (outline != null && outline != currentOutline)
			{
				outline.enabled = true;
				currentOutline = outline;
			}
		}
		else if (currentOutline != null)
		{
			currentOutline.enabled = false;
			currentOutline = null;
		}
	}
}
