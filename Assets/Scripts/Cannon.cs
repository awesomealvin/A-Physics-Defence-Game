using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour {

	[SerializeField]
	private GameObject debugCircle;

	[SerializeField]
	private Text debugText;

	public Transform targetPos;

	[SerializeField]
	private float timeToActivate = 3f;
	private float startTime;
	private bool isDisabled = true;

	// [State = Rotating]
	private bool isRotating = false;
	// [State = Reloading]
	private bool isReloading = false;
	[SerializeField]
	// The time (in seconds) it takes to reload a cannon ball after shooting
	private float reloadSpeed = 2f;
	[SerializeField]
	// The "stall" delay (in seconds). For pauses inbetween things such as shooting and rotating
	private float stallTime = 0.75f;
	//[HideInInspector]
	// The boolean that controls the stalling [State = Stalling]
	public bool isStalling = false;

	[SerializeField]
	private readonly float defaultForce = 20f;

	[SerializeField]
	// Maximum angle the barrel could rotate to (for the rng)
	private float maxAngle = 89f;
	[SerializeField]
	// Minimum angle the barrel could rotate to (for the rng)
	private float minAngle = 0f;
	// The rotation that the barrel should rotate towards
	private Quaternion targetRotation;
	[SerializeField]
	// Speed of the rotation
	private float rotationSpeed = 20f;

	private GameObject[] targets;
	private GameObject selectedTarget;
	private Vector2 selectedTargetPosition;
	// [State = Targeting]
	private bool isTargeting = false;

	[SerializeField]
	private Transform barrel;
	[SerializeField]
	private Transform shootPoint;

	// [State = Shooting]
	private bool isShooting = false;

	[SerializeField]
	// All the cannonball prefabs
	private GameObject[] cannonBallPrefabs;
	[SerializeField]
	[Tooltip("Is a list, but will be converted to a queue during runtime")]
	private List<CannonBall.CannonBallType> cannonBallListQueue;
	private Queue<GameObject> cannonBallQueue;

	[SerializeField]
	private Text ammoCountText;
	private int maxAmmoCount;
	private int currentAmmoCount;

	// Used to check if the cannon is currently rotating
	Quaternion previousRotation;

	// Use this for initialization
	void Awake() {
		previousRotation = barrel.rotation;

		// Uses the ballista's method (because i'm lazy to recode) to convert the list in the
		// inspector to the queue. Because queue's aren't available to be shown in the inspector :(
		cannonBallQueue = Ballista.ListToGameObjectQueue<CannonBall.CannonBallType>(cannonBallListQueue, cannonBallPrefabs);

	}

	void Start() {
		maxAmmoCount = cannonBallQueue.Count;
		UpdateAmmoTextCount();
		if (isDisabled) {
			StartCoroutine("Enable", timeToActivate);
		}
	}

	IEnumerator Enable() {
		yield return new WaitForSeconds(timeToActivate);
		isDisabled = false;
		isTargeting = true;
	}

	// Update is called once per frame
	void Update() {
		if (isDisabled) {
			return;
		}

		if (isTargeting) {
			SetDebugText("Targeting");
			Target();
		}

		if (isRotating) {
			SetDebugText("Rotating");
			RotateBarrel();
		}

		if (isStalling) {
			SetDebugText("Stalling");
			Stall();
		}

		if (isShooting) {
			SetDebugText("Shooting");
			Shoot(PrepareShot());
		}

		if (isReloading) {
			SetDebugText("Reloading");
			Reload();
		}

		// if (!isReloading) {
		// 	RotateBarrel();
		// }

		// // If it ain't rotating, has reloaded, and is not stalled, then it can shoot...
		// // Could put the statement in the shooting function, but could add other stuff in this
		// // section later on...
		// if (!isRotating && !isReloading && !isStalling) {
		// 	Shoot(PrepareShot());
		// 	PrepareTarget();
		// }
	}

	void Target() {
		isTargeting = false;
		SearchForTargets("Block");
		selectedTarget = SelectRandomTarget();
		// If there's no more targets available, then disable the cannon...
		if (selectedTarget == null) {
			isDisabled = true;
			return;
		}
		// Generates a new target rotation for the barrel
		SetTargetRotation(selectedTarget);
		selectedTargetPosition = SelectRandomTargetPosition(selectedTarget);
		isRotating = true;
	}

	void Stall() {
		isStalling = false;
		StartCoroutine("Continue");
	}

	void Reload() {
		isReloading = false;
		StartCoroutine("Reloading");
	}

	/// <summary>
	/// Prepares the shot by calculating the force needed, and the target position
	/// </summary>
	/// <returns>The force calculated by the CalculateForce() function</returns>
	float PrepareShot() {
		float force = defaultForce;
		if (cannonBallQueue.Count > 0) {
			Rigidbody2D nextCannonBallRb = cannonBallQueue.Peek().GetComponent<Rigidbody2D>();
			force = CalculateForce(shootPoint.position, selectedTargetPosition, barrel.eulerAngles.z, nextCannonBallRb);
		}
		return force;
	}

	/// <summary>
	/// Uses the 4 equations of motion for a projectile, or the four projectile motion formulas? idk
	/// Anyways, uses some physics formulas to calculate the force (initial velocity).
	/// </summary>
	/// <param name="source">Position of where the projectile is being shot from</param>
	/// <param name="target">The target position where the projectile should land</param>
	/// <param name="angle">The angle in degrees (or Z value) the projectile should launch at</param>
	/// <param name="projectileRb">The rigidbody2D component of the projectile for the mass and gravity scale</param>
	/// <returns>float value of the calculated force</returns>
	float CalculateForce(Vector2 source, Vector2 target, float angle, Rigidbody2D projectileRb) {
		float gravity = Physics2D.gravity.magnitude * projectileRb.gravityScale;
		Vector2 difference = target - source;

		// Get the angle and convert to radians
		float radians = angle * Mathf.Deg2Rad;

		// Find the opposite and adjacent from the angle
		float opposite = Mathf.Sin(radians); // y
		float adjacent = Mathf.Cos(radians); // x

		// Horizontal distance between source and target
		float xDistance = difference.x;

		/**
		 * From here on, the names of these variables probably won't make sense.
		 * I just converted it to code from using Projectile Motion Formulas...
		 */

		float xComponent = xDistance / adjacent;
		float yComponent = opposite * xComponent;

		float xTime = Mathf.Pow(xComponent, 2);
		xTime = xTime * (gravity / 2);

		yComponent += difference.y;

		// force or initial velocity, same thing
		float force = Mathf.Sqrt((-xTime) / yComponent);
		// apply mass scale
		force *= projectileRb.mass;
		return force;
	}

	/// <summary>
	/// Shoots whatever is left in the queue with the desired force
	/// </summary>
	/// <param name="force">The amount of force to apply</param> 
	void Shoot(float force) {
		if (cannonBallQueue.Count > 0) {
			GameObject nextCannonBall = cannonBallQueue.Dequeue();
			GameObject projectile = Instantiate(nextCannonBall, shootPoint.position, Quaternion.identity);
			projectile.GetComponent<Rigidbody2D>().AddForce(barrel.right * force, ForceMode2D.Impulse);
			//projectile.GetComponent<Rigidbody2D>().velocity = barrel.right*force;

			// Sets the start and target position for the projectile scoring system
			EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
			enemyProjectile.SetScoringPositions(shootPoint.position, selectedTargetPosition);

			// Sets shooting state to false
			isShooting = false;

			// Sets it so it can't shoot again unless reloaded
			isReloading = true;

			// Update Ammo Count Text
			UpdateAmmoTextCount();
		}
	}

	/// <summary>
	///	Sets the target rotation by randomzing an angle 
	/// between the max and min angles, then sets the targetRotation (Quaternion)
	/// </summary>
	private void SetTargetRotation(GameObject target) {

		// Randomize the angle
		float angle = Random.Range(minAngle, maxAngle);

		#region Prevents the randomized angle to be lower than the target
		Vector2 targetPosition = target.transform.position;
		Vector2 barrelPosition = barrel.position;
		// Adds the offset for the shootpoint
		barrelPosition.y += shootPoint.position.y;
		/**
		 * Adds the length of the target object to to target position's y
		 * This makes it guarenteed that the angle of the barrel will
		 * always be greater than the target.
		 */
		Collider2D collider = target.GetComponent<Collider2D>();
		if (collider != null) {
			Bounds bounds = collider.bounds;
			Vector2 maxBounds = bounds.max;
			Vector2 minBounds = bounds.min;
			float length = Vector2.Distance(maxBounds, minBounds);
			targetPosition.y += length;
		}
		/**
		 * Makes sure target angle is never lower that the target
		 */
		// Gets the difference to find the angle
		Vector2 difference = targetPosition - (Vector2) barrelPosition;
		// Calculates the angle
		float targetAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		// Flips the angle if the cannon is also flipped
		float flipOffset = transform.eulerAngles.y;
		targetAngle = flipOffset - targetAngle;

		/**
		 * Sets it so the angle is never lower than the target
		 * and is also never greater than straight up (90 deg)
		 */
		angle = (angle < targetAngle) ? targetAngle : angle;
		angle = (angle > maxAngle) ? maxAngle : angle;
		#endregion
		// Sets the target rotation by converting the euler angle
		// Don't know why I need the 180f on the Y axis, but it works this way...
		targetRotation = Quaternion.Euler(0f, 180f, angle);
	}

	/// <summary>
	/// Rotates the barrel towards the target rotation.
	/// Also sets the isRotating boolean depending whether the
	/// rotation has reached the target rotation or not.
	/// </summary>
	private void RotateBarrel() {
		if (!isStalling) {
			barrel.rotation = Quaternion.RotateTowards(barrel.rotation, targetRotation, Time.deltaTime * rotationSpeed);
			if (barrel.rotation == targetRotation) {
				isRotating = false;
				isStalling = true;
			} else {
				isRotating = true;
			}
		}
	}

	/// <summary>
	/// Reloads the cannon after the defined time (reloadSpeed)
	/// </summary>
	IEnumerator Reloading() {
		yield return new WaitForSeconds(reloadSpeed);
		isTargeting = true;
	}

	/// <summary>
	/// Continues the cannon so that it is not stalled after the defined time (stall time)
	/// </summary>
	/// <param name="time">Time in seconds it takes to continue</param>
	IEnumerator Continue() {
		yield return new WaitForSeconds(stallTime);
		isShooting = true;
	}

	void SearchForTargets(string tag) {
		targets = GameObject.FindGameObjectsWithTag(tag);
	}

	GameObject SelectRandomTarget() {
		GameObject target = null;
		if (targets.Length > 0) {
			int index = Random.Range(0, targets.Length - 1);
			target = targets[index];
		}
		return target;
	}

	/// <summary>
	/// Selects a position within a GameObject using the collider bounds
	/// for the cannon to shoot at
	/// </summary>
	/// <param name="target"></param>
	/// <returns>Vector2 position</returns>
	Vector2 SelectRandomTargetPosition(GameObject target) {
		Collider2D collider = target.GetComponent<Collider2D>();
		if (collider != null) {
			Bounds bounds = collider.bounds;
			Vector2 max = bounds.max;
			Vector2 min = bounds.min;

			float x = Random.Range(min.x, max.x);
			float y = Random.Range(min.y, max.y);

			Vector2 position = new Vector2(x, y);
			return position;
		}

		return target.transform.position;
	}


	private void UpdateAmmoTextCount() {
		currentAmmoCount = cannonBallQueue.Count;
		if (ammoCountText != null) {
			ammoCountText.text = currentAmmoCount+"/"+maxAmmoCount;
		}
	}

	private void DebugPosition(Vector2 position) {
		Instantiate(debugCircle, position, Quaternion.identity);
	}

	void SetDebugText(string text) {
		if (debugText != null) {
			debugText.text = "Cannon is " + text;
		}
	}

}