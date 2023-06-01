using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class LineOfSightController : Singleton<LineOfSightController>
{
	[SerializeField] private LayerMask wallMask;

	private readonly List<Dictionary<NetworkBehaviour, Entry>> teams = new();

	private void LateUpdate()
	{
		// Go through each team, actors.Count = team count
		for (int i = 0; i < teams.Count; i++)
		{
			var sourceTeam = teams[i];

			for (int j = 0; j < teams.Count; j++)
			{
				// Don't test against same team members
				if (i == j) continue;

				var targetTeam = teams[j];

				foreach (var source in sourceTeam)
				{
					foreach (var target in targetTeam)
					{
						if (source.Key == null || target.Key == null) 
							continue;

						var sourceCanSeeTarget = IsInSight(target.Value.lineOfSightTargets, source.Value.eyePosition.position);
						if (sourceCanSeeTarget)
							source.Value.visible.Add(target.Key);
						else
							source.Value.visible.RemoveAll(x => x == target.Key);
					}
				}
			}
		}
	}

	private bool IsInSight(List<LineOfSightTarget> targets, Vector3 origin)
	{
		bool didHit = false;
		foreach (var target in targets)
		{
			if (Check(target.transform.position))
				didHit = true;
		}
		return didHit;

		bool Check(Vector3 posToCheck)
		{
			var distToTarget = Vector3.Distance(origin, posToCheck);
			var ray = new Ray(origin, (posToCheck - origin).normalized);
			if (Physics.Raycast(ray, out var hit, 100f, wallMask))
			{
				if (hit.distance < distToTarget)
				{
					Debug.DrawLine(origin, posToCheck, Color.yellow);
					return false;
				}

				Debug.DrawLine(origin, posToCheck, Color.green);
				return true;
			}

			Debug.DrawLine(origin, posToCheck, Color.green);
			return true;
		}
	}

	public void Register(NetworkBehaviour nb, ActorController ac)
	{
		var no = nb.NetworkObject;
		int team = GetTeamId(no);

		if (team >= 0)
		{
			while (teams.Count <= team)
				teams.Add(new Dictionary<NetworkBehaviour, Entry>());

			var entry = new Entry
			{
				eyePosition = ac.EyePosition,
				collider = no.GetComponent<Collider>(),
				lineOfSightTargets = ListPool<LineOfSightTarget>.Get(),
				visible = ListPool<NetworkBehaviour>.Get()
			};

			entry.lineOfSightTargets.AddRange(ac.LineOfSightTargets);
			teams[team].Add(nb, entry);
		}
	}

	public void Unregister(NetworkBehaviour nb)
	{
		if (!nb.IsSpawned) return;

		int team = GetTeamId(nb.NetworkObject);
		if (team < 0 || team >= teams.Count)
			return;

		var entry = teams[team][nb];
		ListPool<LineOfSightTarget>.Release(entry.lineOfSightTargets);
		ListPool<NetworkBehaviour>.Release(entry.visible);
		teams[team].Remove(nb);
	}

	public void GetVisible(NetworkBehaviour pov, List<NetworkBehaviour> visible)
	{
		visible.Clear();

		var entry = teams[GetTeamId(pov.NetworkObject)][pov];
		foreach (var v in entry.visible)
			if (Is.NotNull(v))
				visible.Add(v);
	}

	public bool IsVisible(NetworkBehaviour pov, NetworkBehaviour target)
	{
		var entry = teams[GetTeamId(pov.NetworkObject)][pov];
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
		public Transform eyePosition;
		public Collider collider;
		public List<LineOfSightTarget> lineOfSightTargets;
		public List<NetworkBehaviour> visible;
	}
}
