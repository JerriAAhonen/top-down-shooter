using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SyncedPrefabDb : ScriptableObject
{
	[SerializeField] private GameObject[] systems;
	[SerializeField] private Actors actors;

	public IReadOnlyList<GameObject> Systems => systems;
	
	public void GetActorPrefabs(PlayerSpawnData data, out GameObject actorPrefab)
	{
		actorPrefab = actors.roots[data.rootPrefab];
	}

	[Serializable]
	public class Actors
	{
		public GameObject[] roots;
		public GameObject[] visuals;
	}
	
#if UNITY_EDITOR
	private void OnValidate()
	{
		var all = new List<GameObject>();
		all.AddRange(systems);
		all.AddRange(actors.roots);
		all.AddRange(actors.visuals);
		all.RemoveAll(Is.Null);
		all.RemoveDuplicates();

		string guid = UnityEditor.AssetDatabase.FindAssets("t:NetworkPrefabsList")[0];
		string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
		var list = UnityEditor.AssetDatabase.LoadAssetAtPath<NetworkPrefabsList>(path);

		foreach (var p in new List<NetworkPrefab>(list.PrefabList))
			list.Remove(p);

		foreach (var p in all)
			list.Add(new NetworkPrefab { Prefab = p });

		UnityEditor.EditorUtility.SetDirty(list);
	}
#endif
}
