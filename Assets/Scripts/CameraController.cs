using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float offsetThreshold;

	private Vector3 aimMidPoint;

	private void LateUpdate()
	{
		var targetPos = aimMidPoint + offset;
		transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * movementSpeed);
	}

	public void SetMousePos(Vector3 mousePos)
	{
		var midPoint = (player.position + mousePos) / 2f;

		midPoint.x = Mathf.Clamp(midPoint.x, player.position.x - offsetThreshold, player.position.x + offsetThreshold);
		midPoint.z = Mathf.Clamp(midPoint.z, player.position.z - offsetThreshold, player.position.z + offsetThreshold);

		aimMidPoint = midPoint;
	}
}
