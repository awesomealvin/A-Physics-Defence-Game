using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballista : MonoBehaviour {

	[SerializeField]
	private Transform flightGroove;
	[SerializeField]
	private Transform shootPoint;

	[SerializeField]
	private float maxShootForce = 10;
	[SerializeField]
	private GameObject[] arrowPrefabs;
	[SerializeField]
	private bool unlimitedAmmo = false;

	private bool isAiming;

	// Arrow Queue to be converted to an actaul queue.
	// Is a list because it can be serialized
	[SerializeField]
	private List<ArrowType> arrowQueue;
	private Queue<ArrowType> arrows;

	private Rigidbody2D rb;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		arrows = ArrowListToQueue(arrowQueue);
	}

	/**
	 * Converts the arrows list to a queue
	 */
	private Queue<ArrowType> ArrowListToQueue(List<ArrowType> list) {
		Queue<ArrowType> newQueue = new Queue<ArrowType>();
		foreach (ArrowType a in list) {
			newQueue.Enqueue(a);
		}

		return newQueue;
	}

	public void Aim(Vector2 firstPos, Vector2 secondPos) {
		// Gets the difference between the two points
		Vector2 difference = firstPos - secondPos;

		// Sets the initial click rotation
		if (firstPos.Equals(secondPos)) {
			difference = firstPos - (Vector2) flightGroove.transform.position;
		}

		// Calculates the angle from the two points
		float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

		// Applies rotation as Quaternion
		flightGroove.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	/// <summary>
	/// Shoots out the next arrow in the ammunition queue.
	/// Called from the Controller class
	/// </summary>
	/// <param name="forcePercentage">Percentage of the maximum shoot force to apply</param>
	public void Shoot(float forcePercentage) {
		if (arrows.Count > 0) {
			GameObject nextArrow;
			// Dequeue if unlimited arrow = true
			if (unlimitedAmmo) {
				nextArrow = arrowPrefabs[(int) arrows.Peek()];
			} else {
				nextArrow = arrowPrefabs[(int) arrows.Dequeue()];
			}

			// Instantiates the arrow
			GameObject projectile = Instantiate(nextArrow, shootPoint.position, Quaternion.identity);
			// Calculates the force percentage
			float force = forcePercentage * maxShootForce;
			// Adds the force to the instantiated projectile
			projectile.GetComponent<Rigidbody2D>().AddForce(flightGroove.right * force, ForceMode2D.Impulse);

		} else {
			Debug.Log("No more arrows!");
		}

	}
}