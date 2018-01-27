using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ballista))]
public class BallistaController : MonoBehaviour {

	[SerializeField]
	private Ballista ballista;

	[SerializeField]
	private float maxDrawLength;
	private float currentDrawLength;

	[SerializeField]
	private Text lengthPercentageText;

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
			initialTouchPos = GetCurrentMousePos();
			
			
		}

		if (isMouseDown) {
			currentTouchPos = GetCurrentMousePos();
			ballista.Aim(initialTouchPos, currentTouchPos);
			CalculateLength();
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
		}
	}

	private Vector2 GetCurrentMousePos() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	private void CalculateLength() {
		currentDrawLength = (currentTouchPos - initialTouchPos).magnitude;
		currentDrawLength = (currentDrawLength > maxDrawLength)?maxDrawLength:currentDrawLength;
		float _percentage = currentDrawLength / maxDrawLength;
		
		lengthPercentageText.text = (_percentage*100f)+"%";
	}
}