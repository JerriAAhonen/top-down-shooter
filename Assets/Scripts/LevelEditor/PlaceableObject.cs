using System;
using UnityEngine;

public enum PlaceableObjectCategory { Wall, Door, Window }

[CreateAssetMenu]
public class PlaceableObject : ScriptableObject
{
    [SerializeField] private PlaceableObjectCategory category;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;

    public PlaceableObjectCategory Category => category;
    public Sprite Icon => icon;
	public GameObject Prefab => prefab;
}
