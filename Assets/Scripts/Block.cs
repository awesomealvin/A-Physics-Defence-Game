using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	// test
	public int points = 1000;
	[SerializeField]
	private GameManager gameManager;

	public bool debug = false;

	private int blockDamage;

	[SerializeField]
	private int maxHealth = 100;
	private int currentHealth;
	[SerializeField]
	private Color highHealthColor;
	private readonly int highHealthValue = 75;
	[SerializeField]
	private Color midHealthColor;
	private readonly int midHealthValue = 50;
	[SerializeField]
	private Color lowHealthColor;
	private readonly int lowHealthValue = 15;
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float minProjectileVelocityDamageTolerance = 3f;
	[SerializeField]
	private float maxProjectileVelocityDamageTolerance = 15f;

	[SerializeField]
	private float minEnvironmentVelocityDamageTolerance = 3f;
	[SerializeField]
	private float maxEnvironmentVelocityDamageTolerance = 50f;

	void Awake() {
		blockDamage = maxHealth;
		currentHealth = maxHealth;

		gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

		// Fixes the issue where the colors are transparent
		highHealthColor.a = 1f;
		midHealthColor.a = 1f;
		lowHealthColor.a = 1f;
	}

	void OnCollisionEnter2D(Collision2D other) {
		// if (!debug) {
		// 	return;
		// }
		Vector2 velocity = other.relativeVelocity;
		if (other.gameObject.CompareTag("Enemy Projectile")) {
			int damage = other.gameObject.GetComponent<EnemyProjectile>().GetDamage();
			float percentage = CalculateDamagePercentage(minProjectileVelocityDamageTolerance,
				maxProjectileVelocityDamageTolerance, velocity);
			Damage(CalculateDamageFromPercentage(damage, percentage));
		} else if (other.gameObject.CompareTag("Block") ||
			other.gameObject.CompareTag("Platform")) {
			float percentage = CalculateDamagePercentage(minEnvironmentVelocityDamageTolerance,
				maxEnvironmentVelocityDamageTolerance, velocity);
			Damage(CalculateDamageFromPercentage(blockDamage, percentage));
		}
	}

	int CalculateDamageFromPercentage(int damage, float percentage) {
		float newDamage = damage * percentage;
		return Mathf.RoundToInt(newDamage);
	}

	float CalculateDamagePercentage(float minVelocityTolerance, float maxVelocityTolerance, Vector2 velocity) {
		float magnitude = velocity.magnitude;
		if (magnitude < minVelocityTolerance) {
			return 0f;
		} else {
			float totalTolerance = maxVelocityTolerance - minVelocityTolerance;
			float percentage = magnitude / totalTolerance;
			percentage = (percentage > 1f) ? 1f : percentage;
//			Debug.Log(percentage);

			return percentage;
		}
	}

	void Damage(int damage) {
		if (damage <= 0) {
			return;
		}
		currentHealth -= damage;
		if (debug) {
			Debug.Log("Current Health = " + currentHealth);
		}

		if (currentHealth <= lowHealthValue) {
			spriteRenderer.color = lowHealthColor;
		} else if (currentHealth <= midHealthValue) {
			spriteRenderer.color = midHealthColor;
		} else if (currentHealth <= highHealthValue) {
			spriteRenderer.color = highHealthColor;
		}

		if (currentHealth <= 0) {
			DestroyBlock();
		 }
	}

	void DestroyBlock() {
		Destroy(gameObject);
		if (gameManager != null) {
			gameManager.DeductPoints(points);
		}
	}
}