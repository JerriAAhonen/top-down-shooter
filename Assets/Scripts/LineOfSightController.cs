using System.Collections.Generic;
using UnityEngine;

public class LineOfSightController : Singleton<LineOfSightController>
{
	private readonly HashSet<LineOfSightTarget> registered = new();
	private LineOfSightSource source;
	private Transform sourceTm;

	private void LateUpdate()
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
	}

	private bool IsInSight(LineOfSightTarget target, Vector3 sourcePos)
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
	}

	public void Register(LineOfSightSource source)
	{
		this.source = source;
		sourceTm = source.transform;
	}

	public void Unregister(LineOfSightSource source)
	{
		if (this.source == source)
		{
			this.source = null;
			sourceTm = null;
		}
	}

	public void Register(LineOfSightTarget losv)
	{
		registered.Add(losv);
	}

	public void Unregister(LineOfSightTarget losv)
	{
		registered.Remove(losv);
	}
}
