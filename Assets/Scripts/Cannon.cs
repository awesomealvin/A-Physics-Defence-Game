using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	public Transform targetPos;

	private bool isRotating = false;
	private bool hasReloaded = true;
	[SerializeField]
	// The time (in seconds) it takes to reload a cannon ball after shooting
	private float reloadSpeed = 2f;
	[SerializeField]
	// The "stall" delay (in seconds). For pauses inbetween things such as shooting and rotating
	private float stallTime = 0.75f;
	//[HideInInspector]
	// The boolean that controls the stalling
	public bool isStalled = false;

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

	[SerializeField]
	private Transform barrel;
	[SerializeField]
	private Transform shootPoint;

	[SerializeField]
	// All the cannonball prefabs
	private GameObject[] cannonBallPrefabs;
	[SerializeField]
	[Tooltip("Is a list, but will be converted to a queue during runtime")]
	private List<CannonBall.CannonBallType> cannonBallQueue;
	private Queue<CannonBall.CannonBallType> cannonBalls;

	// Used to check if the cannon is currently rotating
	Quaternion previousRotation;

	// Use this for initialization
	void Start() {
		previousRotation = barrel.rotation;
		targetRotation = barrel.rotation;

		// Uses the ballista's method (because i'm lazy to recode) to convert the list in the
		// inspector to the queue. Because queue's aren't available to be shown in the inspector :(
		cannonBalls = Ballista.ListToQueue<CannonBall.CannonBallType>(cannonBallQueue);
	}

	// Update is called once per frame
	void Update() {
		RotateBarrel();

		// If it ain't rotating, has reloaded, and is not stalled, then it can shoot...
		// Could put the statement in the shooting function, but could add other stuff in this
		// section later on...
		if (!isRotating && hasReloaded && !isStalled) {
			float gravity = Physics2D.gravity.magnitude;
			Vector3 target = targetPos.position;
			Vector3 source = shootPoint.position;

			Vector3 difference = target - source;

			// Get the angle and convert to radians
			float angle = barrel.eulerAngles.z;
			float radians = angle * Mathf.Deg2Rad;

			// Find the opposite and adjacent from the angle
			float opposite = Mathf.Sin(radians); // y
			float adjacent = Mathf.Cos(radians); // x

			// Horizontal distance between source and target
			float xDistance = difference.x;
			
			
			float xComponent = xDistance / adjacent;
			float yComponent = opposite * xComponent;

			float xTime = Mathf.Pow(xComponent, 2);
			xTime = xTime * (gravity/2);

			yComponent += difference.y;

			float force = Mathf.Sqrt((-xTime)/yComponent);

			Shoot(force);

		}
	}

	/// <summary>
	/// Shoots whatever is left in the queue with the desired force
	/// </summary>
	/// <param name="force">The amount of force to apply</param> 
	void Shoot(float force) {
		if (cannonBalls.Count > 0) {
			GameObject nextCannonBall = cannonBallPrefabs[(int) cannonBalls.Dequeue()];
			GameObject projectile = Instantiate(nextCannonBall, shootPoint.position, Quaternion.identity);
			projectile.GetComponent<Rigidbody2D>().AddForce(barrel.right * force, ForceMode2D.Impulse);
			//projectile.GetComponent<Rigidbody2D>().velocity = barrel.right*force;

			// Sets it so it can't shoot again unless reloaded
			hasReloaded = false;
			// Pauses the cannon for a bit by "stalling?" it
			isStalled = true;

			// Generates a new target rotation for the barrel
			SetTargetRotation();
			// Resets the isStalled boolean after a delay
			StartCoroutine("Continue");
			// Resets the hasReloaded boolean so that it can shoot again
			StartCoroutine("Reload");
		}
	}

	/// <summary>
	///	Sets the target rotation by randomzing an angle 
	/// between the max and min angles, then sets the targetRotation (Quaternion)
	/// </summary>
	private void SetTargetRotation() {

		// Randomize the angle
		float angle = Random.Range(minAngle, maxAngle);

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
		if (!isStalled) {
			barrel.rotation = Quaternion.RotateTowards(barrel.rotation, targetRotation, Time.deltaTime * rotationSpeed);
			if (barrel.rotation == targetRotation) {
				isRotating = false;
			} else {
				isRotating = true;
			}
		}
	}

	/// <summary>
	/// Reloads the cannon after the defined time (reloadSpeed)
	/// </summary>
	IEnumerator Reload() {
		yield return new WaitForSeconds(reloadSpeed);
		hasReloaded = true;
	}

	/// <summary>
	/// Continues the cannon so that it is not stalled after the defined time (stall time)
	/// </summary>
	IEnumerator Continue() {
		yield return new WaitForSeconds(stallTime);
		isStalled = false;
	}

}