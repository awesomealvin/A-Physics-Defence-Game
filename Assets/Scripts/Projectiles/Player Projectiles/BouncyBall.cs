using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class BouncyBall : Ball {
	
	float angle;
	Vector2 difference;

	[SerializeField]
	private float slowDownScaleFactor = 0.25f;

	override public void UseAbilityOnTouch(Vector2 initialTouchPosition) {
		this.initialTouchPosition = initialTouchPosition;
		Time.timeScale = slowDownScaleFactor;
		Time.fixedDeltaTime = Time.timeScale *0.02f;
	}

	override public void UseAbilityOnHold(Vector2 currentTouchPosition) {
		this.currentTouchPosition = currentTouchPosition;
		difference =  initialTouchPosition -currentTouchPosition;
		angle = BallistaController.CalculateAngle(initialTouchPosition, currentTouchPosition, transform.position);

		
	}	

	override public void UseAbilityOnRelease() {
		rb.velocity = new Vector2(0f, 0f);
		Shoot();
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
	}

	public void Shoot() {
		rb.AddForce(difference.normalized * 20f, ForceMode2D.Impulse);
	}

	
	
}
