using Unity.Netcode;
using UnityEngine;

public class ActorShooting : NetworkBehaviour
{
	[SerializeField] private ActorWeapons aw;
	[SerializeField] private Transform shootPoint;
	[SerializeField] private ParticleSystem projectilePS;
	[SerializeField] private ParticleSystem casingPS;
	[SerializeField] private ParticleSystem muzzleFlashPS;
	[SerializeField] private ParticleSystem muzzleSmokePS;

	private bool shootPressed;

	private void Update()
	{
		if (!NetworkObject.IsLocalPlayer)
			return;

		Vector3 from = shootPoint.position;
		Vector3 to = CursorController.Instance.AimPoint;
		Vector3 direction = to - from;
		shootPoint.rotation = Quaternion.LookRotation(direction);
	}

	public void OnShootPressed(bool pressed)
	{
		shootPressed = pressed;
	}

	public void OnReloadPressed()
	{
		if (aw.CurrentWeapon == null) 
			return;
		aw.CurrentWeapon.OnReload();
	}

	public void Process()
	{
		if (aw.CurrentWeapon == null)
			return;

		aw.CurrentWeapon.OnShoot(shootPressed, Time.fixedDeltaTime, OnShot);

		void OnShot()
		{
			var output = aw.CurrentWeapon.WeaponOutput;
			if (output.ProjectileCount > 1)
			{
				for (int i = 0; i < output.ProjectileCount; i++)
				{
					var randX = Random.Range(-output.MaxScatterAngle, output.MaxScatterAngle);
					var randY = Random.Range(-output.MaxScatterAngle, output.MaxScatterAngle);
					var dir = shootPoint.forward + new Vector3(randX, randY, 0f);
					ExecuteMultiShot(dir, i == output.ProjectileCount - 1);
				}
			}
			else
			{
				Vector3 from = shootPoint.position;
				Vector3 to = CursorController.Instance.AimPoint;
				Vector3 direction = to - from;

				// Only execute the shot here for clients, since host's execution happens later.
				if (!NetworkObject.IsOwnedByServer)
					ExecuteSingleShot(from, direction);

				Shoot_ServerRpc(from, direction);
			}
		}
	}

	private void ExecuteSingleShot(Vector3 _, Vector3 __)
	{
		projectilePS.Play();
		casingPS.Play();
		muzzleFlashPS.Play();
		muzzleSmokePS.Play();
	}

	private void ExecuteMultiShot(Vector3 dir, bool isLast)
	{
		projectilePS.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
		projectilePS.Emit(1);

		if (isLast)
		{
			projectilePS.Play();
			casingPS.Play();
			muzzleFlashPS.Play();
			muzzleSmokePS.Play();
		}
	}

	[ServerRpc(RequireOwnership = true)]
	private void Shoot_ServerRpc(Vector3 from, Vector3 direction, ServerRpcParams serverRpcParams = default)
	{
		ExecuteSingleShot(from, direction);
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
			ExecuteSingleShot(from, direction);
	}
}
