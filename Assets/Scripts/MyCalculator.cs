using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyCalculator {

	public static float CalculateAngle(Vector2 firstPos, Vector2 secondPos, Vector2 position) {
		// Gets the difference between the two points
		Vector2 difference = firstPos - secondPos;

		// Sets the initial click rotation
		if (firstPos.Equals(secondPos)) {
			difference = firstPos - (Vector2) position;
		}

		// Calculates the angle from the two points
		float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		//		Debug.Log(angle);
		return angle;
	}

	public static float CalcuateForcePercentage(Vector2 initialPos, Vector2 currentPos, float maxLength) {
		Vector2 difference = initialPos - currentPos;
		float length = difference.magnitude;
		float percentage = length / maxLength;
		percentage = (percentage > 1f) ? 1f : percentage;
		return percentage;
	}

	public static Vector2 CalculateDirectionFromTwoPoints(Vector2 firstPos, Vector2 secondPos) {
		return (firstPos - secondPos).normalized;
	}

}
