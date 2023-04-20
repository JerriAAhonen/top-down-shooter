using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrefabDb : ScriptableObject
{
	[SerializeField] private GameObject[] actors;
	[SerializeField] private GameObject[] actorVisuals;
	
	public void GetActorPrefabs(PlayerSpawnData data, out GameObject actorPrefab, out GameObject visualsPrefab)
	{
		actorPrefab = actors[data.actorPrefab];
		visualsPrefab = actorVisuals[data.visualsPrefab];
	}
	
	private void OnValidate()
	{
		NetworkPrefabsList list = null;

		var all = new List<GameObject>();
		all.AddRange(actors);
		all.AddRange(actorVisuals);
		all.RemoveAll(Is.Null);
		all.RemoveDuplicates();

		string guid = UnityEditor.AssetDatabase.FindAssets("t:NetworkPrefabsList")[0];
		string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
		list = UnityEditor.AssetDatabase.LoadAssetAtPath<NetworkPrefabsList>(path);

		foreach (var p in new List<NetworkPrefab>(list.PrefabList))
			list.Remove(p);

		foreach (var p in all)
			list.Add(new NetworkPrefab { Prefab = p });

		UnityEditor.EditorUtility.SetDirty(list);
		UnityEditor.AssetDatabase.SaveAssets();
	}
}
