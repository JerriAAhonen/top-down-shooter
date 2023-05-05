using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clutter : MonoBehaviour
{
	[SerializeField] private Transform modelRoot;
	[SerializeField] private Rigidbody mainRigidBody;
	[SerializeField] private Collider mainCollider;

	private int health;
	private List<Rigidbody> partRigidBodies = new();
	private List<Collider> partColliders = new();

	public Rigidbody RigidBody => mainRigidBody;

	private void Awake()
	{
		health = 10;
		foreach (Transform child in modelRoot)
		{
			var partRb = child.GetComponent<Rigidbody>();
			partRb.isKinematic = true;
			partRigidBodies.Add(partRb);
			
			var partCol = child.GetComponent<Collider>();
			partCol.enabled = false;
			partColliders.Add(partCol);
		}
	}

	public bool Damage(int damage)
	{
		health -= damage;
		return health < 0;
	}

	public void Break(float force, Vector3 forcePos, float explosionRadius)
	{
		mainRigidBody.isKinematic = true; 
		mainCollider.enabled = false;

		foreach (var partCol in partColliders)
		{
			partCol.enabled = true;
		}

		foreach (var partRb in partRigidBodies)
		{
			partRb.isKinematic = false;
			partRb.AddExplosionForce(force, forcePos, explosionRadius);
		}

		StartCoroutine(DespawnRoutine());
	}

	private IEnumerator DespawnRoutine()
	{
		yield return new WaitForSeconds(5);

		var duration = 5f;
		var elapsed = 0f;
		while (elapsed < duration)
		{
			yield return null;
			elapsed += Time.deltaTime;

			var scale = 1 - (elapsed / duration);
			foreach (var partCol in partColliders)
				partCol.transform.localScale = Vector3.one * scale;
		}

		Destroy(gameObject);
	}
}
