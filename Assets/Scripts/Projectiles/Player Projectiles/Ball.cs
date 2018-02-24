using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ball : PlayerProjectile {

	public abstract void UseAbilityOnTouch();
	public abstract void UseAbilityOnHold();
	public abstract void UseAbilityOnRelease();

}