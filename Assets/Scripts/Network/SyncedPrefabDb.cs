using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SyncedPrefabDb : ScriptableObject
{
	[SerializeField] private GameObject[] systems;
	[SerializeField] private GameObject[] players;
	[SerializeField] private GameObject[] enemies;

	public IReadOnlyList<GameObject> Systems => systems;
	public GameObject GetPlayer(PlayerSpawnData data) => players[data.prefab];
	public GameObject GetEnemy(EnemySpawnData data) => enemies[data.prefab];

#if UNITY_EDITOR
	private void OnValidate()
	{
		string[] guids = UnityEditor.AssetDatabase.FindAssets("t:NetworkPrefabsList");
		if (guids.Length == 0)
			return;

		string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
		var list = UnityEditor.AssetDatabase.LoadAssetAtPath<NetworkPrefabsList>(path);
		if (!list)
			return;

		var all = new List<GameObject>();
		all.AddRange(systems);
		all.AddRange(players);
		all.AddRange(enemies);
		all.RemoveAll(Is.Null);
		all.RemoveDuplicates();

		foreach (var p in new List<NetworkPrefab>(list.PrefabList))
			list.Remove(p);

		foreach (var p in all)
			list.Add(new NetworkPrefab { Prefab = p });

		UnityEditor.EditorUtility.SetDirty(list);
	}
#endif
}
