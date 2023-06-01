using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

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
			var from = shootPoint.position;

			var directions = new Vector3[output.ProjectileCount];
			for (int i = 0; i < output.ProjectileCount; i++)
			{
				var randX = Random.Range(-output.MaxScatterAngle, output.MaxScatterAngle);
				var randY = Random.Range(-output.MaxScatterAngle, output.MaxScatterAngle);
				var dir = Quaternion.Euler(randX, randY, 0) * shootPoint.forward;
				directions[i] = dir;

				// Only execute the shot here for clients, since host's execution happens later.
				if (!NetworkObject.IsOwnedByServer)
					ExecuteShot(from, dir, i == output.ProjectileCount - 1);
			}

			Shoot_ServerRpc(from, directions);
		}
	}

	private void ExecuteShot(Vector3 from, Vector3 dir, bool isLast)
	{
		projectilePS.transform.position = from;
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
	private void Shoot_ServerRpc(Vector3 from, Vector3[] directions, ServerRpcParams serverRpcParams = default)
	{
		for (int i = 0; i < directions.Length; i++)
		{
			ExecuteShot(from, directions[i], i == directions.Length - 1);

			var ray = new Ray(from, directions[i]);
			if (Physics.Raycast(ray, out var hit, 100f))
			{
				var ah = hit.collider.GetComponentInParent<ActorHealth>();
				if (ah)
					ah.DealDamage(5);
			}
		}

		Shoot_ClientRpc(from, directions);
	}

	[ClientRpc]
	private void Shoot_ClientRpc(Vector3 from, Vector3[] directions)
	{
		if (!NetworkObject.IsOwner)
		{
			// Execute shot only if not owned (owner's execution happens instantly when shooting)
			for (int i = 0; i < directions.Length; i++)
				ExecuteShot(from, directions[i], i == directions.Length - 1);
		}
	}
}
