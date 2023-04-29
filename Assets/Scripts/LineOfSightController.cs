using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class LineOfSightController : Singleton<LineOfSightController>
{
	private readonly List<Dictionary<NetworkBehaviour, Entry>> actors = new();

	/*private void LateUpdate()
	{
		bool hasSource = source != null;
		Vector3 sourcePos = hasSource ? sourceTm.position : default;
		Vector3 sourceDir = (hasSource ? sourceTm.forward : default).SetY(0).normalized;
		float fov = hasSource ? source.FieldOfView / 2 : 0;

		foreach (var target in registered)
		{
			if (!hasSource)
			{
				target.SetVisible(false);
				continue;
			}

			Vector3 sourceToTarget = target.Transform.position - sourcePos;
			Debug.DrawLine(sourcePos, sourcePos + sourceDir * 100, Color.white);

			if (sourceToTarget.sqrMagnitude > target.Range * target.Range)
			{
				target.SetVisible(false);
				continue;
			}

			if (Vector3.Angle(sourceToTarget.SetY(0).normalized, sourceDir) <= fov)
			{
				target.SetVisible(IsInSight(target, sourcePos));
				continue;
			}

			target.SetVisible(false);
		}
	}*/

	/*private bool IsInSight(LineOfSightTarget target, Vector3 sourcePos)
	{
		bool didHit = Check(Vector3.zero);
		didHit = Check(Vector3.left) || didHit;
		didHit = Check(Vector3.right) || didHit;
		didHit = Check(Vector3.up) || didHit;
		didHit = Check(Vector3.down) || didHit;
		return didHit;

		bool Check(Vector3 offset)
		{
			Vector3 origin = target.Collider.bounds.center;
			origin += offset;
			
			var ray = new Ray(origin, (sourcePos - origin).normalized);
			if (Physics.Raycast(ray, out var hit, 100f) && hit.collider == source.Collider)
			{
				Debug.DrawLine(origin, sourcePos, Color.green);
				return true;
			}
			
			Debug.DrawLine(origin, sourcePos, Color.yellow);
			return false;
		}
	}*/

	public void Register(NetworkBehaviour nb)
	{
		var no = nb.NetworkObject;
		int team = GetTeamId(no);

		if (team >= 0)
		{
			while (actors.Count <= team)
				actors.Add(new Dictionary<NetworkBehaviour, Entry>());

			var entry = new Entry
			{
				transform = no.transform,
				collider = no.GetComponent<Collider>(),
				visible = ListPool<NetworkBehaviour>.Get()
			};
			actors[team].Add(nb, entry);
		}
	}

	public void Unregister(NetworkBehaviour nb)
	{
		int team = GetTeamId(nb.NetworkObject);
		if (team < 0 || team >= actors.Count)
			return;

		var entry = actors[team][nb];
		ListPool<NetworkBehaviour>.Release(entry.visible);
		actors[team].Remove(nb);
	}

	public void GetVisible(NetworkBehaviour pov, List<NetworkBehaviour> visible)
	{
		visible.Clear();

		var entry = actors[GetTeamId(pov.NetworkObject)][pov];
		foreach (var v in entry.visible)
			if (Is.NotNull(v))
				visible.Add(v);
	}

	public bool IsVisible(NetworkBehaviour pov, NetworkBehaviour target)
	{
		var entry = actors[GetTeamId(pov.NetworkObject)][pov];
		return entry.visible.Contains(target);
	}

	private int GetTeamId(NetworkObject no)
	{
		if (no.IsPlayerObject)
			return 0;
		return 1;
	}

	private class Entry
	{
		public Transform transform;
		public Collider collider;
		public List<NetworkBehaviour> visible;
	}
}
