using UnityEngine;

public class CameraController : Singleton<CameraController>
{
	[SerializeField] private Vector3 offset;
	[SerializeField] private float movementSpeed;
	[SerializeField] private float offsetThreshold;

	private Transform playerTm;
	private Vector3 aimMidPoint;

	private void LateUpdate()
	{
		var targetPos = aimMidPoint + offset;
		transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * movementSpeed);
	}

	public void Init(Transform playerTm)
	{
		this.playerTm = playerTm;
	}

	public void SetMousePos(Vector3 mousePos)
	{
		if (!playerTm)
			return;

		var playerPos = playerTm.position;
		var midPoint = (playerPos + mousePos) / 2f;
		midPoint.x = Mathf.Clamp(midPoint.x, playerPos.x - offsetThreshold, playerPos.x + offsetThreshold);
		midPoint.z = Mathf.Clamp(midPoint.z, playerPos.z - offsetThreshold, playerPos.z + offsetThreshold);

		aimMidPoint = midPoint;
	}
}
