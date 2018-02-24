using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : Projectile {

	[SerializeField]
	public int maxPoints;
	[SerializeField]
	public int minPoints;
	//[HideInInspector]
	public Vector2 startPosition;
	//[HideInInspector]
	public Vector2 targetPosition;
	[SerializeField]
	public float minPointsDistancePercentage = 0.3f;

	private bool hasHit = false;

	[SerializeField]
	// Damage is based on the mass
	readonly float maxDamageMultiplier = 20f;

	protected virtual void ContactWithPlayerProjectile() {
		Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Player Projectile")) {
			ContactWithPlayerProjectile();
		} 
		if (other.gameObject.CompareTag("Block")) {
			hasHit = true;
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

	public void SetScoringPositions(Vector2 startPosition, Vector2 targetPosition) {
		this.startPosition = startPosition;
		this.targetPosition = targetPosition;
	}


	public int GetCurrentPoints() {
		// Horizontal Values
		float startX = startPosition.x;
		float targetX = targetPosition.x;
		float currentX = transform.position.x;

		// Original Distance Values
		float maxDistance = Mathf.Abs(targetX - startX);
		float currentDistance = Mathf.Abs(targetX - currentX);
		float minDistance = minPointsDistancePercentage * maxDistance;

		// Distsance Values with the min percentage and points offset
		float newMaxPoints = maxPoints - minPoints;
		float newCurrentDistance = currentDistance - minDistance;
		float newMaxDistance = maxDistance - minDistance;

		// Caluclate distance percentage between offsetted min, max and current
		float currentPercentage = newCurrentDistance / newMaxDistance;
		currentPercentage = (currentPercentage < 0f) ? 0f : currentPercentage;

		// Return points with minPoints added to correct the offset
		int points = Mathf.RoundToInt(currentPercentage * newMaxPoints) + minPoints;

		// If the cannon has already hit a block, then player can no longer earn points from it
		if (hasHit) {
			points = 0;
		}
		return points;
	}

}