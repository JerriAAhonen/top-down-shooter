using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LineOfSightTarget : NetworkBehaviour
{
	[SerializeField] private Optional<float> range;

	private List<MeshRenderer> mr;
	private List<SkinnedMeshRenderer> smr;
	private List<LineRenderer> lr;
	private bool isVisible = true;

	public override void OnNetworkSpawn()
	{
		var no = GetComponentInParent<NetworkObject>();
		if (no.IsPlayerObject && no.IsOwner)
		{
			enabled = false;
			return;
		}
		
		Transform = no.transform;
		Collider = no.GetComponent<Collider>();

		PoolUtil.Get(ref mr);
		PoolUtil.Get(ref smr);
		PoolUtil.Get(ref lr);
		LineOfSightController.Instance.Register(this);
	}

	public override void OnDestroy()
	{
		LineOfSightController.Instance.Unregister(this);
		PoolUtil.Release(ref mr);
		PoolUtil.Release(ref smr);
		PoolUtil.Release(ref lr);
		base.OnDestroy();
	}

	public Transform Transform { get; private set; }
	public Collider Collider { get; private set; }
	public float Range => range.HasValue ? range.Value : 10f;

	public void SetVisible(bool visible)
	{
		if (isVisible == visible)
			return;

		isVisible = visible;

		SetVisibility(mr);
		SetVisibility(smr);
		SetVisibility(lr);
	}

	private void SetVisibility<T>(List<T> components)
		where T : Renderer
	{
		Transform.GetComponentsInChildren(components);
		foreach (var t in components)
			t.enabled = isVisible;
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		float radius = Range;

		using (new TemporaryHandlesMatrix(transform))
		{
			using (new TemporaryHandlesColor(Color.red.Opacity(0.03f)))
			{
				UnityEditor.Handles.DrawSolidDisc(Vector3.zero, Vector3.up, radius);
			}

			using (new TemporaryHandlesColor(Color.red))
			{
				UnityEditor.Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius);
			}
		}
	}
#endif
}
