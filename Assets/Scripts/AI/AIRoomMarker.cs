using UnityEngine;

public class AIRoomMarker : MonoBehaviour
{
	[SerializeField] private float radius = 1;

	public Vector3 GetRandomPosition()
	{
		var angle = Quaternion.Euler(0, Random.Range(0, 360), 0);
		return transform.position + angle * (Vector3.forward * radius);
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		using (new TemporaryHandlesMatrix(transform))
		{
			using (new TemporaryHandlesColor(Color.yellow.Opacity(0.03f)))
			{
				UnityEditor.Handles.DrawSolidDisc(Vector3.zero, Vector3.up, radius);
			}

			using (new TemporaryHandlesColor(Color.yellow))
			{
				UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius);
			}
		}
	}
#endif
}
