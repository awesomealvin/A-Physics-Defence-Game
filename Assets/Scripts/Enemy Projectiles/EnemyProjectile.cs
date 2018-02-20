using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour {

	[SerializeField]
	// Damage is based on the mass
	readonly float maxDamageMultiplier = 10f;

	protected virtual void ContactWithPlayerProjectile() {
		Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player Projectile")) {
			ContactWithPlayerProjectile();
		}
	}

	public int GetDamage() {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if (rb != null) {
			return Mathf.RoundToInt(rb.mass * maxDamageMultiplier);
		} else {
			return Mathf.RoundToInt(maxDamageMultiplier);
		}
	}
	
}
