using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour {

	protected virtual void ContactWithPlayerProjectile() {
		Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player Projectile")) {
			ContactWithPlayerProjectile();
		}
	}
	
}
