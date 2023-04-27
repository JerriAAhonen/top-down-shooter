using Unity.Netcode;
using UnityEngine;

public class ActorShooting : NetworkBehaviour
{
	[SerializeField] private Weapon currentWeapon;
	[SerializeField] private Transform shootPoint;
	[SerializeField] private ParticleSystem projectilePS;
	[SerializeField] private ParticleSystem casingPS;
	[SerializeField] private ParticleSystem muzzleFlashPS;
	[SerializeField] private ParticleSystem muzzleSmokePS;

	private bool shootPressed;

	public void OnShootPressed(bool pressed)
	{
		shootPressed = pressed;
	}

	public void OnReloadPressed()
	{
		currentWeapon?.OnReload();
	}

	public void Process()
	{
		currentWeapon?.OnShoot(shootPressed, Time.fixedDeltaTime, OnShot);

		void OnShot()
		{
			Vector3 from = shootPoint.position;
			Vector3 direction = transform.forward;

			// Only execute the shot here for clients, since host's execution happens later.
			if (!NetworkObject.IsOwnedByServer)
				ExecuteShot(from, direction);

			Shoot_ServerRpc(from, direction);
		}
	}

	private void ExecuteShot(Vector3 from, Vector3 direction)
	{
		projectilePS.Play();
		casingPS.Play();
		muzzleFlashPS.Play();
		muzzleSmokePS.Play();
	}

	[ServerRpc(RequireOwnership = true)]
	private void Shoot_ServerRpc(Vector3 from, Vector3 direction, ServerRpcParams serverRpcParams = default)
	{
		ExecuteShot(from, direction);
		Shoot_ClientRpc(from, direction);

		var ray = new Ray(from, direction);
		if (Physics.Raycast(ray, out var hit, 100f))
		{
			var ah = hit.collider.GetComponentInParent<ActorHealth>();
			if (ah)
				ah.DealDamage(5);
		}
	}

	[ClientRpc]
	private void Shoot_ClientRpc(Vector3 from, Vector3 direction)
	{
		// Execute shot only if not owned (owner's execution happens instantly when shooting)
		if (!NetworkObject.IsOwner)
			ExecuteShot(from, direction);
	}
}
