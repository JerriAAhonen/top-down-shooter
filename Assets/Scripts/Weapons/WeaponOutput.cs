using UnityEngine;

public class WeaponOutput : MonoBehaviour
{
	[SerializeField] private int projectileCount;
	[SerializeField] private float maxScatterAngle;

	public int ProjectileCount => projectileCount;
	public float MaxScatterAngle => maxScatterAngle;
}
