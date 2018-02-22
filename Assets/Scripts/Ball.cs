using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ball : MonoBehaviour {

	private GameManager gameManager;

	void Awake() {
		gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
	}

	public abstract void UseAbility();

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Enemy Projectile")) {
			if (gameManager != null) {
				EnemyProjectile projectileHit = other.gameObject.GetComponent<EnemyProjectile>();
				gameManager.AddPoints(projectileHit.GetCurrentPoints());
			}
		}
	}
}