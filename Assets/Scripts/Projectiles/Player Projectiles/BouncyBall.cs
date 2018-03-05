using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : Ball {

	// Time it takes to smooth (smooth damp)
	private readonly float cameraSmoothTime = 0.025f;
	private Vector3 currentSmoothVelocity = Vector3.zero;

	// Smooth speeds (Lerp)
	private readonly float cameraSmoothSpeed = 15f;
	private readonly float cameraSmoothSpeed2 = 7.5f;

	float angle;
	Vector2 difference;

	[SerializeField]
	private float maxDrawLength = 3f;
	[SerializeField]
	private float maxForce = 40f;
	private float currentForcePercentage;

	[SerializeField]
	private ProjectileArc projectileArc;

	[SerializeField]
	private float slowDownScaleFactor = 0.10f;

	[SerializeField]
	private float zoomPercentage = 0.15f;
	private float defaultCameraSize;
	private float desiredCameraSize;

	Vector3 defaultCameraPosition;

	bool touched = false;

	Coroutine cameraFollowBall;
	Coroutine cameraZoom;

	override protected void FixedUpdate() {
		base.FixedUpdate();
		if (touched) {
			CameraFollowBall();
		}
	}

	void StopCoroutines() {
		if (cameraFollowBall != null && cameraZoom != null) {
			StopCoroutine(cameraFollowBall);
			StopCoroutine(cameraZoom);
		}

	}

	override public void UseAbilityOnTouch(Vector2 initialTouchPosition) {
		touched = true;
		this.initialTouchPosition = initialTouchPosition;
		Time.timeScale = slowDownScaleFactor;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
		defaultCameraPosition = Camera.main.transform.position;
		defaultCameraSize = Camera.main.orthographicSize;
		desiredCameraSize = defaultCameraSize - (zoomPercentage * defaultCameraSize);
	}

	override public void UseAbilityOnHold(Vector2 currentTouchPosition) {
		this.currentTouchPosition = currentTouchPosition;
		angle = MyCalculator.CalculateAngle(initialTouchPosition, currentTouchPosition, transform.position);
		currentForcePercentage = MyCalculator.CalcuateForcePercentage(initialTouchPosition, currentTouchPosition, maxDrawLength);
		if (abilityCharges > 0f) {
			if (projectileArc != null) {
				projectileArc.UpdateProjectileArc(rb, transform.position, angle, currentForcePercentage * maxForce);
			}
		}

		CameraZoom();
	}

	override public void UseAbilityOnRelease() {
		touched = false;
		Shoot();
		if (projectileArc != null) {
			projectileArc.DisableArc();
		}
		ResetTimeScale();
		cameraFollowBall = StartCoroutine(ResetCameraPosition());
		cameraZoom = StartCoroutine(ResetCameraZoom());
	}

	void ResetTimeScale() {
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
	}

	public void Shoot() {
		if (abilityCharges <= 0f) {
			return;
		}
		rb.velocity = new Vector2(0f, 0f);
		float force = currentForcePercentage * maxForce;
		Vector2 direction = MyCalculator.CalculateDirectionFromTwoPoints(initialTouchPosition, currentTouchPosition);
		rb.AddForce(direction * force, ForceMode2D.Impulse);
		DeductCharges();
	}

	private void DeductCharges() {
		abilityCharges--;
		abilityCharges = (abilityCharges < 0f) ? 0f : abilityCharges;
	}

	protected override void OnCollisionEnter2D(Collision2D other) {
		base.OnCollisionEnter2D(other);
		if (other.gameObject.CompareTag("Enemy Projectile")) {
			abilityCharges++;
		} else {
			DeductCharges();
		}

	}

	protected override void DestroyGameObject() {
		ResetTimeScale();
		cameraFollowBall = StartCoroutine(ResetCameraPosition());
		cameraZoom = StartCoroutine(ResetCameraZoom());
		if (Camera.main.transform.position == defaultCameraPosition) {
			base.DestroyGameObject();
		}
	}

	void CameraFollowBall() {
		Camera camera = Camera.main;
		Vector3 cameraPosition = camera.transform.position;
		Vector3 desiredPosition = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
		camera.transform.position = Vector3.SmoothDamp(cameraPosition, desiredPosition, ref currentSmoothVelocity, cameraSmoothTime);
	}

	IEnumerator ResetCameraPosition() {
		Camera camera = Camera.main;
		while (camera.transform.position != defaultCameraPosition) {
			camera.transform.position = Vector3.Lerp(camera.transform.position, GameManager.defaultCameraPosition, Time.deltaTime * cameraSmoothSpeed2);
			yield return null;
		}
	}

	void CameraZoom() {
		Camera camera = Camera.main;
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, desiredCameraSize, Time.deltaTime * cameraSmoothSpeed);
	}

	IEnumerator ResetCameraZoom() {
		Camera camera = Camera.main;
		while (camera.orthographicSize != defaultCameraSize) {
			camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, GameManager.defaultCameraSize, Time.deltaTime * cameraSmoothSpeed2);
			yield return null;
		}
	}

	public override void Unfocus() {
		StopCoroutines();
	}

}