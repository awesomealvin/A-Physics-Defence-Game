using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	public bool debug = false;

	private int blockDamage;

	[SerializeField]
	private int maxHealth = 100;
	private int currentHealth;
	[SerializeField]
	private Color midHealthColor;
	private readonly int midHealthValue = 50;
	[SerializeField]
	private Color lowHealthColor;
	private readonly int lowHealthValue = 15;
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float minProjectileVelocityDamageTolerance = 6f;
	[SerializeField]
	private float maxProjectileVelocityDamageTolerance = 15f;

	[SerializeField]
	private float minPlatformVelocityDamageTolerance = 5f;
	[SerializeField]
	private float maxPlatformVelocityDamageTolerance = 40f;

	[SerializeField]
	private float minBlockVelocityDamageTolerance = 3f;
	[SerializeField]
	private float maxBlockVelocityDamageTolerance = 40f;

	void Awake() {
		blockDamage = maxHealth;
		currentHealth = maxHealth;

		// Fixes the issue where the colors are transparent
		midHealthColor.a = 1f;
		lowHealthColor.a = 1f;
	}

	void OnCollisionEnter2D(Collision2D other) {
		// if (!debug) {
		// 	return;
		// }
		Vector2 velocity = other.relativeVelocity;
		if (other.gameObject.CompareTag("Enemy Projectile")) {
			int damage = other.gameObject.GetComponent<EnemyProjectile>().maxDamage;
			float percentage = CalculateDamagePercentage(minProjectileVelocityDamageTolerance,
				maxProjectileVelocityDamageTolerance, velocity);

			Damage(CalculateDamageFromPercentage(damage, percentage));
		} else if (other.gameObject.CompareTag("Block")) {

		} else if (other.gameObject.CompareTag("Platform")) {

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
			Debug.Log(percentage);

			return percentage;
		}
	}

	void Damage(int damage) {
		currentHealth -= damage;
		if (debug) {
			Debug.Log("Current Health = " + currentHealth);

		}
		if (currentHealth <= midHealthValue) {
			spriteRenderer.color = midHealthColor;
		}

		if (currentHealth <= lowHealthValue) {
			spriteRenderer.color = lowHealthColor;
		}

		if (currentHealth <= 0) {
			Destroy(gameObject);
		}
	}
}