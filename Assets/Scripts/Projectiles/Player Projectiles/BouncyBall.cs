using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : Ball {

	float angle;
	Vector2 difference;

	[SerializeField]
	private float maxDrawLength = 3f;
	[SerializeField]
	private float maxForce = 40f;
	private float currentForcePercentage;

	[SerializeField]
	private ProjectileArc projectileArc;

	[SerializeField]
	private float slowDownScaleFactor = 0.15f;

	override public void UseAbilityOnTouch(Vector2 initialTouchPosition) {
		this.initialTouchPosition = initialTouchPosition;
		Time.timeScale = slowDownScaleFactor;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
	}

	override public void UseAbilityOnHold(Vector2 currentTouchPosition) {
		this.currentTouchPosition = currentTouchPosition;
		angle = MyCalculator.CalculateAngle(initialTouchPosition, currentTouchPosition, transform.position);
		currentForcePercentage = MyCalculator.CalcuateForcePercentage(initialTouchPosition, currentTouchPosition, maxDrawLength);

		if (abilityCharges > 0f) {
			if (projectileArc != null) {
				projectileArc.UpdateProjectileArc(rb, transform.position, angle, currentForcePercentage * maxForce);
			}
		}

	}

	override public void UseAbilityOnRelease() {
		Shoot();
		if (projectileArc != null) {
			projectileArc.DisableArc();
		}
		ResetTimeScale();
	}

	void ResetTimeScale() {
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
	}

	public void Shoot() {
		if (abilityCharges <= 0f) {
			return;
		}
		rb.velocity = new Vector2(0f, 0f);
		float force = currentForcePercentage * maxForce;
		Vector2 direction = MyCalculator.CalculateDirectionFromTwoPoints(initialTouchPosition, currentTouchPosition);
		rb.AddForce(direction * force, ForceMode2D.Impulse);
		DeductCharges();
	}

	private void DeductCharges() {
		abilityCharges--;
		abilityCharges = (abilityCharges < 0f) ? 0f : abilityCharges;
	}

	protected override void OnCollisionEnter2D(Collision2D other) {
		base.OnCollisionEnter2D(other);
		if (other.gameObject.CompareTag("Enemy Projectile")) {
			abilityCharges++;
		} else {
			DeductCharges();
		}

	}

	protected override void DestroyGameObject() {
		base.DestroyGameObject();
		ResetTimeScale();
	}

}