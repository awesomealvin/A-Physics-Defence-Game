using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ballista))]
public class BallistaController : MonoBehaviour {

	[SerializeField]
	private Ballista ballista;

	private bool isMouseDown;

	private Vector2 initialTouchPos;
	private Vector2 currentTouchPos;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

		if (Input.GetMouseButtonDown(0)) {
			isMouseDown = true;
			initialTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			
		}

		if (isMouseDown) {

		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
		}
	}

}