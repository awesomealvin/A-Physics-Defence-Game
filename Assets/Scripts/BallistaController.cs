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

/* ENABLE WHEN RELEASED FOR WINDOWS
#if UNITY_STANDALONE_WIN		
		if (Input.GetMouseButtonDown(0)) {
			// Checks if the shoot area has been touched
			hasTouchedShootArea = IsOnShootTouchArea(GetCurrentMousePos());
			// Checks if the ability area has been touched
			hasTouchedAbilityArea = IsOnAbilityTouchArea(GetCurrentMousePos());

			if (hasTouchedAbilityArea) {
				ballista.UseLastBallAbility();
			}

			isMouseDown = true;
			initialTouchPos = GetCurrentMousePos();
		}
#endif
 */


#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS

		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began) {
				// Checks if the shoot area has been touched
				hasTouchedShootArea = IsOnShootTouchArea(GetCurrentTouchPos());
				// Checks if the ability area has been touched
				hasTouchedAbilityArea = IsOnAbilityTouchArea(GetCurrentTouchPos());

				if (hasTouchedAbilityArea) {
					ballista.UseLastBallAbility();
				}

				isMouseDown = true;
				initialTouchPos = GetCurrentTouchPos();
			}
		}

#endif

		if (isMouseDown) {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
			currentTouchPos = GetCurrentTouchPos();
#endif

#if UNITY_STANDALONE_WIN
			currentTouchPos = GetCurrentMousePos();
#endif

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


/* DISABLED WHEN RELEASED ON WINDOWS
#if UNITY_STANDALONE_WIN

		if (Input.GetMouseButtonUp(0)) {
			isMouseDown = false;

			/// <summary>
			/// Abilities
			/// </summary>
			if (hasTouchedAbilityArea) {

			}

			/// <summary>
			/// Shooting
			/// </summary>
			if (hasTouchedShootArea) {
				ballista.Shoot(currentDrawLength / maxDrawLength);
				EnableLengthPercentageText(false);
			}

		}
#endif
*/

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0) {
			if (Input.GetTouch(0).phase == TouchPhase.Ended) {
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
#endif
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

	private Vector2 GetCurrentTouchPos() {
		if (Input.touchCount > 0) {
			return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
		}

		return new Vector2(0f, 0f);
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