using System.Collections;
using System.Collections.Generic;
using tds.Input;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float movementSpeed;
	[SerializeField] float rotationSpeed;

	private CharacterController cc;
	private Vector3 velocity;
	private Camera cam;
	private Plane mousePosPlane;

	private void Awake()
	{
		cc = GetComponent<CharacterController>();
		cam = Camera.main;
		mousePosPlane = new Plane(Vector3.up, Vector3.zero);
	}

	private void Update()
	{
		PlayerMovement();
		PlayerRotation();
	}

	private void PlayerMovement()
	{
		var movementInput = InputManager.Instance.MovementInput;
		var movement = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
		velocity = movementSpeed * Time.deltaTime * movement;
		cc.Move(velocity);
	}

	private void PlayerRotation()
	{
		var ray = cam.ScreenPointToRay(InputManager.Instance.MousePosition);
		if (mousePosPlane.Raycast(ray, out var intersectionDist))
		{
			var point = ray.GetPoint(intersectionDist);
			var target = new Vector3(point.x, transform.position.y, point.z);
			var rotation = Quaternion.LookRotation(target - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
		}
	}

	private void OnDrawGizmos()
	{
		Debug.DrawRay(transform.position, transform.forward * 20f);
	}
}
