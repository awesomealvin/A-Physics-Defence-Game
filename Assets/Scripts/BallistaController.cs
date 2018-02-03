using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ballista))]
public class BallistaController : MonoBehaviour {

	[SerializeField]
	private Ballista ballista;

	[SerializeField]
	private float maxDrawLength = 5f;
	private float currentDrawLength;

	[SerializeField]
	private Text lengthPercentageText;

	private bool isMouseDown;

	private Vector2 initialTouchPos;
	private Vector2 currentTouchPos;

	// Use this for initialization
	void Start() {
		EnableLengthPercentageText(false);
	}

	// Update is called once per frame
	void Update() {

		if (Input.GetMouseButtonDown(0)) {
			isMouseDown = true;
			initialTouchPos = GetCurrentMousePos();
			EnableLengthPercentageText(true);
			
		}

		if (isMouseDown) {
			currentTouchPos = GetCurrentMousePos();
			ballista.Aim(initialTouchPos, currentTouchPos);
			CalculateLength();
		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;
			ballista.Shoot(currentDrawLength/maxDrawLength);
			EnableLengthPercentageText(false);
			
		}
	}

	private Vector2 GetCurrentMousePos() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	private void CalculateLength() {
		currentDrawLength = (currentTouchPos - initialTouchPos).magnitude;
		currentDrawLength = (currentDrawLength > maxDrawLength)?maxDrawLength:currentDrawLength;
		float percentage = currentDrawLength / maxDrawLength;
		
		lengthPercentageText.text = (percentage*100f)+"%";
	}

	private void EnableLengthPercentageText(bool value) {
		if (lengthPercentageText != null) {
			lengthPercentageText.enabled = value;
		}
	}
}