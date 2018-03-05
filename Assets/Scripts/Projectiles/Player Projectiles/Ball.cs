using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ball : PlayerProjectile {

	protected Vector2 initialTouchPosition;
	protected Vector2 currentTouchPosition;

	[SerializeField]
	protected float abilityCharges = 1f;

	public abstract void UseAbilityOnTouch(Vector2 initialTouchPosition);
	public abstract void UseAbilityOnHold(Vector2 currentTouchPosition);
	public abstract void UseAbilityOnRelease();

	public abstract void Unfocus();
}