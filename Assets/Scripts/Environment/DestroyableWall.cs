using System.Collections.Generic;
using UnityEngine;

public class DestroyableWall : MonoBehaviour
{
	public void ApplyHits(IReadOnlyList<Hit> hits)
	{
		foreach (var h in hits)
			Debug.DrawLine(h.point, h.point + h.normal * 0.3f, Color.red, 2);
	}

	public struct Hit
	{
		public Vector3 point;
		public Vector3 normal;
	}
}
