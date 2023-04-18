using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float movementSpeed;

	private void Update()
	{
		transform.position = Vector3.Slerp(transform.position, player.position + offset, Time.deltaTime * movementSpeed);
	}
}
