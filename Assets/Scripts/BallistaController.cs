using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ballista))]
public class BallistaController : MonoBehaviour {

	[SerializeField]
	private RawImage shootTouchArea;
	bool hasTouchedShootArea = false;

	[SerializeField]
	private RawImage abilityTouchArea;
	bool hasTouchedAbilityArea = false;

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
		ManageTouches();
	}

	bool HasTouchAreas() {
		if (shootTouchArea != null && abilityTouchArea != null) {
			return true;
		}
		return false;
	}

	void ManageTouches() {
		if (Input.GetMouseButtonDown(0)) {
			// Checks if the shoot area has been touched
			hasTouchedShootArea = IsOnShootTouchArea(GetCurrentMousePos());
			// Checks if the ability area has been touched
			hasTouchedAbilityArea = IsOnAbilityTouchArea(GetCurrentMousePos());

			isMouseDown = true;
			initialTouchPos = GetCurrentMousePos();
		}

		if (isMouseDown) {
			currentTouchPos = GetCurrentMousePos();

			/**
			 * Abilities
			 */
			if (hasTouchedAbilityArea) {

			}

			/**
			 * Shooting
			 */
			if (hasTouchedShootArea) {
				EnableLengthPercentageText(true);
				ballista.Aim(initialTouchPos, currentTouchPos);
				CalculateLength();
			}

		}

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;

			/**
			 * Abilities
			 */
			if (hasTouchedAbilityArea) {

			}

			/**
			 * Shooting
			 */
			if (hasTouchedShootArea) {
				ballista.Shoot(currentDrawLength / maxDrawLength);
				EnableLengthPercentageText(false);
			}

		}
	}

	bool IsOnShootTouchArea(Vector2 touchPos) {
		if (RectTransformUtility.RectangleContainsScreenPoint(shootTouchArea.rectTransform, touchPos)) {
			Debug.Log("Touched Shooting Area");
			return true;
		}
		return false;
	}

	bool IsOnAbilityTouchArea(Vector2 touchPos) {
		if (RectTransformUtility.RectangleContainsScreenPoint(abilityTouchArea.rectTransform, touchPos)) {
			Debug.Log("Touched Ability Area");
			return true;
		}
		return false;
	}

	private Vector2 GetCurrentMousePos() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	private void CalculateLength() {
		currentDrawLength = (currentTouchPos - initialTouchPos).magnitude;
		currentDrawLength = (currentDrawLength > maxDrawLength) ? maxDrawLength : currentDrawLength;
		float percentage = currentDrawLength / maxDrawLength;

		lengthPercentageText.text = (percentage * 100f) + "%";
	}

	private void EnableLengthPercentageText(bool value) {
		if (lengthPercentageText != null) {
			lengthPercentageText.enabled = value;
		}
	}
}