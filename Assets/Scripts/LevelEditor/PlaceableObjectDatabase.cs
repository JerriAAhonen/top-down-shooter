using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class PlaceableObjectDatabase : ScriptableObject
{
	[SerializeField] private List<PlaceableObject> placeableObjects;

	public IReadOnlyList<PlaceableObject> GetObjects(PlaceableObjectCategory category)
	{
		return placeableObjects.Where(obj => obj.Category == category).ToList();
	}

	public int GetId(PlaceableObject obj) => obj != null ? placeableObjects.IndexOf(obj) : -1;
	public PlaceableObject GetObject(int id) => placeableObjects[id];
}
