using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class BouncyBall : Ball {
	
	[SerializeField]
	private float slowDownScaleFactor = 0.25f;

	override public void UseAbilityOnTouch() {
		Time.timeScale = slowDownScaleFactor;
		Time.fixedDeltaTime = Time.timeScale *0.02f;
	}

	override public void UseAbilityOnHold() {

	}

	override public void UseAbilityOnRelease() {
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
	}

	
}
