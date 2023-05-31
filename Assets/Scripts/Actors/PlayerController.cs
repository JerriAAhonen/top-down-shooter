using tds.Input;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : ActorController
{
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (!NetworkObject.IsOwner)
			return;
		
		PlayerMovement();
		PlayerRotation();
		shooting.Process();
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		MainRpc.Instance.RegisterPlayer(this);

		if (NetworkObject.IsOwner)
		{
			CameraController.Instance.Init(transform);
			FieldOfViewController.Instance.SetTarget(transform);

			InputManager.Instance.Shoot += shooting.OnShootPressed;
			InputManager.Instance.Reload += shooting.OnReloadPressed;
		}
	}

	public override void OnDestroy()
	{
		MainRpc.Instance?.UnregisterPlayer(this);
		base.OnDestroy();
	}

	private void PlayerMovement()
	{
		var movementInput = InputManager.Instance.MovementInput;
		var movement = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
		animationVelocity = movementSpeed * Time.deltaTime * movement;
		rb.AddForce(animationVelocity, ForceMode.VelocityChange);
	}

	private void PlayerRotation()
	{
		if (!CursorController.Instance.IsReady)
			return;

		var aimPoint = CursorController.Instance.AimPoint;
		var target = aimPoint.SetY(transform.position.y);

		// Since the shootpoint is offset to the side, get the rotation from the shootpoint to the traget
		var relativeRotationPos = shootPoint.position;
		relativeRotationPos.y = 0f;

		var rotation = Quaternion.LookRotation(target - relativeRotationPos);
		rb.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

		CameraController.Instance.SetMousePos(target);

		// Aiming line renderer
		var aimLineTargetX = 0f;
		var aimLineTargetY = aimLine.GetPosition(0).y;
		var aimLineTargetZ = Vector3.Distance(target, shootPoint.position);

		var aimLineTarget = new Vector3(aimLineTargetX, aimLineTargetY, aimLineTargetZ);
		aimLine.SetPosition(1, aimLineTarget);
	}
}
