using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float minHorizontalOffset;
	[SerializeField] private float maxHorizontalOffset;
	[SerializeField] private float movementSpeed;

	private void Update()
	{
		var targetPos = player.position + offset;
		transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * movementSpeed);
	}

}
