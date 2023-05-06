using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using static UnityEngine.UI.Image;

public class FieldOfViewController : Singleton<FieldOfViewController>
{
	[SerializeField] private MeshFilter viewMf;
	[SerializeField] private MeshFilter fogMf;
	[Space]
	[SerializeField] private float viewAltitude = 0.1f;
	[SerializeField] private float fogAltitude = 2f;
	[Space]
	[SerializeField] private int fov = 110;
	[SerializeField] private float viewDistance = 50;
	[SerializeField] private float meshResolution = 0.3f;
	[SerializeField] private int edgeIterations = 3;
	[SerializeField] private LayerMask mask;

	private Transform target;
	private Mesh viewMesh;
	private Mesh fogMesh;

	protected override void Awake()
	{
		base.Awake();

		viewMesh = new Mesh();
		viewMf.sharedMesh = viewMesh;

		fogMesh = new Mesh();
		fogMf.sharedMesh = fogMesh;
	}

	private void LateUpdate()
	{
		viewMesh.Clear();
		fogMesh.Clear();

		if (Is.Null(target)
			|| !target.gameObject.activeInHierarchy
			|| fov <= 0
			|| meshResolution < Mathf.Epsilon)
			return;

		int stepCount = Mathf.RoundToInt(fov * meshResolution);
		float min = -fov / 2f;
		float max = fov / 2f;

		CreateViewMesh(target.position.SetY(viewAltitude), stepCount, min, max);
		CreateFogMesh(target.position.SetY(fogAltitude), stepCount, min, max);
	}

	private void OnDestroy()
	{
		Destroy(viewMesh);
		Destroy(fogMesh);
	}

	public void SetTarget(Transform tm)
	{
		target = tm;
	}

	private void CreateViewMesh(Vector3 origin, int stepCount, float minAngle, float maxAngle)
	{
		using (ListPool<Vector3>.Get(out var points))
		using (ListPool<int>.Get(out var indices))
		{
			points.Add(origin);

			var oldRaycast = new RaycastInfo();

			for (int i = 0; i <= stepCount; i++)
			{
				float angle = Mathf.Lerp(minAngle, maxAngle, i / (float)stepCount);
				var raycast = Raycast(origin, angle, Color.white.Opacity(0.1f));

				if (i > 0 && edgeIterations > 0)
				{
					if (oldRaycast.collider != raycast.collider)
					{
						var edge = FindEdge(origin, oldRaycast, raycast);
						if (!edge.pointA.Approximately(Vector3.zero))
							points.Add(edge.pointA);
						if (!edge.pointB.Approximately(Vector3.zero))
							points.Add(edge.pointB);
					}
				}

				points.Add(raycast.point);
				oldRaycast = raycast;
			}

			for (int i = 1; i < points.Count - 1; i++)
			{
				indices.Add(0);
				indices.Add(i);
				indices.Add(i + 1);
			}

			viewMesh.SetVertices(points);
			viewMesh.SetTriangles(indices, 0);
		}
	}
	private void CreateFogMesh(Vector3 origin, int stepCount, float minAngle, float maxAngle)
	{
		minAngle += 5;
		maxAngle -= 5;

		using (ListPool<Vector3>.Get(out var points))
		using (ListPool<int>.Get(out var indices))
		{
			float dist = viewDistance * 2;

			points.Add(origin);
			points.Add(origin + GetDirection(minAngle, Space.Self) * dist);
			points.Add(origin + GetDirection(180, Space.Self) * dist);
			points.Add(origin + GetDirection(maxAngle, Space.Self) * dist);

			indices.Add(0);
			indices.Add(2);
			indices.Add(1);

			indices.Add(0);
			indices.Add(3);
			indices.Add(2);

			fogMesh.SetVertices(points);
			fogMesh.SetTriangles(indices, 0);
		}
	}

	private Vector3 GetDirection(float angle, Space space)
	{
		if (space == Space.Self)
			angle += target.eulerAngles.y;
		float rad = angle * Mathf.Deg2Rad;
		return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
	}

	private RaycastInfo Raycast(Vector3 origin, float angle, Color color)
	{
		Vector3 dir = GetDirection(angle, Space.Self);

		if (Physics.Raycast(origin, dir, out var hit, viewDistance, mask))
		{
			//Debug.DrawLine(origin, hit.point, color);
			return new RaycastInfo
			{
				collider = hit.collider,
				point = hit.point,
				distance = hit.distance,
				angle = angle
			};
		}

		//Debug.DrawLine(origin, origin + dir * ViewDistance, Color.red);
		return new RaycastInfo
		{
			collider = null,
			point = origin + dir * viewDistance,
			distance = viewDistance,
			angle = angle
		};
	}

	private EdgeInfo FindEdge(Vector3 origin, RaycastInfo min, RaycastInfo max)
	{
		float minAngle = min.angle;
		float maxAngle = max.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			var raycast = Raycast(origin, angle, Color.green);

			if (raycast.collider == min.collider)
			{
				minAngle = angle;
				minPoint = raycast.point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = raycast.point;
			}
		}

		return new EdgeInfo { pointA = minPoint, pointB = maxPoint };
	}

	private struct RaycastInfo
	{
		public Collider collider;
		public Vector3 point;
		public float distance;
		public float angle;
	}

	private struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;
	}
}
