using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : MonoBehaviour {

	[SerializeField]
	private Transform flightGroove;

	[SerializeField]
	private Transform shootPoint;

	private bool isAiming;

	[SerializeField]
	private Queue<GameObject> arrows;

	private Rigidbody2D rb;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		arrows = new Queue<GameObject>();
	}


	public void Aim(Vector2 firstPos, Vector2 secondPos) {
		Vector2 _difference = firstPos - secondPos;

		// Sets the initial click rotation
		if (firstPos.Equals(secondPos)) {
			_difference = firstPos - (Vector2)flightGroove.transform.position;
		}

		float _angle = Mathf.Atan2(_difference.y, _difference.x) * Mathf.Rad2Deg;
		
		flightGroove.rotation = Quaternion.AngleAxis(_angle, Vector3.forward);
	}

	public void Shoot() {

	}
}
