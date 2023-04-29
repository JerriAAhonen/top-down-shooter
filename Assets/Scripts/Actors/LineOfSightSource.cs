using Unity.Netcode;
using UnityEngine;

public class LineOfSightSource : NetworkBehaviour
{
	[SerializeField] private Optional<float> fov;
	
	public override void OnNetworkSpawn()
	{
		var obj = GetComponentInParent<NetworkObject>();
		if (obj.IsPlayerObject && obj.IsOwner)
		{
			Collider = obj.GetComponent<Collider>();
			LineOfSightController.Instance.Register(this);
		}
	}

	public override void OnDestroy()
	{
		LineOfSightController.Instance.Unregister(this);
		base.OnDestroy();
	}

	public Collider Collider { get; private set; }
	public float FieldOfView => fov.HasValue ? fov.Value : 170f;
	
#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		float angle = FieldOfView;
		Vector3 from = Quaternion.Euler(0, -angle / 2f, 0) * Vector3.forward;
		float distance = 2;

		using (new TemporaryHandlesMatrix(transform))
		{
			using (new TemporaryHandlesColor(Color.green.Opacity(0.03f)))
			{
				UnityEditor.Handles.DrawSolidArc(Vector3.zero, Vector3.up, from, angle, distance);
			}

			using (new TemporaryHandlesColor(Color.green))
			{
				UnityEditor.Handles.DrawWireArc(Vector3.zero, Vector3.up, from, angle, distance);
				
				Vector3 left = Quaternion.Euler(0, -angle / 2f, 0) * Vector3.forward * distance;
				Vector3 right = Quaternion.Euler(0, angle / 2f, 0) * Vector3.forward * distance;
				UnityEditor.Handles.DrawLine(Vector3.zero, left);
				UnityEditor.Handles.DrawLine(Vector3.zero, right);
			}
		}
	}
#endif
}
