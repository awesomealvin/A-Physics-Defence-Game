using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : MonoBehaviour {

	private bool isAiming;

	private Rigidbody2D rb;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}


	public void Aim(Vector2 firstPos, Vector2 secondPos) {

	}
}
