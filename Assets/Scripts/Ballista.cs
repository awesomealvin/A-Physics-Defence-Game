using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ballista : MonoBehaviour {

	[SerializeField]
	public Transform flightGroove;
	[SerializeField]
	public Transform shootPoint;

	[SerializeField]
	public float maxShootForce = 40f;
	public float currentForce = 0f;
	[SerializeField]
	private GameObject[] arrowPrefabs;
	[SerializeField]
	private bool unlimitedAmmo = false;

	[SerializeField]
	private Text ammoCountText;
	private int currentAmmoCount;
	private int maxAmmoCount;

	private bool isAiming;

	public Ball lastBall;

	// Arrow Queue to be converted to an actaul queue.
	// Is a list because it can be serialized
	[SerializeField]
	private List<ArrowType> arrowQueueList;
	private Queue<GameObject> arrowQueue;
	public GameObject nextProjectile;

	private Rigidbody2D rb;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		arrowQueue = ListToGameObjectQueue<ArrowType>(arrowQueueList, arrowPrefabs);
		nextProjectile = arrowQueue.Peek();
		maxAmmoCount = arrowQueue.Count;
		currentAmmoCount = maxAmmoCount;
		UpdateAmmoCountText();
	}

	private void UpdateAmmoCountText() {
		currentAmmoCount = arrowQueue.Count;
		if (ammoCountText != null) {
			ammoCountText.text = currentAmmoCount + "/" + maxAmmoCount;
		}
	}

	/**
	 * Converts the arrows list to a queue
	 */
	public static Queue<GameObject> ListToGameObjectQueue<E>(List<E> list, GameObject[] prefabs) {
		Queue<GameObject> newQueue = new Queue<GameObject>();
		foreach (E a in list) {
			GameObject nextItem = prefabs[System.Convert.ToInt32(a)];
			newQueue.Enqueue(nextItem);
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

	public void Aim(float angle) {
		// Applies rotation as Quaternion
		flightGroove.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	/// <summary>
	/// Shoots out the next arrow in the ammunition queue.
	/// Called from the Controller class
	/// </summary>
	/// <param name="forcePercentage">Percentage of the maximum shoot force to apply</param>
	public void Shoot(float forcePercentage) {
		if (arrowQueue.Count > 0) {

			// Sets the focus of the previous ball to unfocused
			if (lastBall != null) {
				SetFocused(lastBall.gameObject, false);
			}

			GameObject nextArrow;
			// Dequeue if unlimited arrow = true
			if (unlimitedAmmo) {
				nextArrow = arrowQueue.Peek();
				arrowQueue.Enqueue(arrowQueue.Dequeue());
			} else {
				nextArrow = arrowQueue.Dequeue();
			}
			nextProjectile = nextArrow;
			// Instantiates the arrow
			GameObject projectile = Instantiate(nextArrow, shootPoint.position, Quaternion.identity);
			// Calculates the force percentage
			float force = forcePercentage * maxShootForce;
			// Adds the force to the instantiated projectile
			projectile.GetComponent<Rigidbody2D>().AddForce(flightGroove.right * force, ForceMode2D.Impulse);

			// Sets focus of this projectile to true
			SetFocused(projectile, true);

			SetLastBall(projectile);

			// Decrease ammo count text
			UpdateAmmoCountText();

		} else {
			Debug.Log("No more arrows!");
		}

	}

	// public void UseLastBallAbility() {
	// 	if (lastBall != null) {
	// 		lastBall.UseAbility();
	// 	}
	// }

	void SetLastBall(GameObject ball) {
		if (lastBall != null) {
			lastBall.Unfocus();
		}
		lastBall = ball.GetComponent<Ball>();
	}

	void SetFocused(GameObject ball, bool focused) {
		PlayerProjectile projectile = ball.GetComponent<PlayerProjectile>();
		if (projectile != null) {
			projectile.focused = focused;
		}
	}
}