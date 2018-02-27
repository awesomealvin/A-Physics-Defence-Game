using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ballista))]
[RequireComponent(typeof(BallistaController))]
public class ProjectileArc : MonoBehaviour {

	[SerializeField]
	private float length;
	[SerializeField]
	private float spacing = 0.75f;
	private int amount;

	[SerializeField]
	GameObject projectileArcPointPrefab;
	GameObject[] projectileArcPoints;

	void Awake() {
		amount = Mathf.RoundToInt(length / spacing);

	}

	private void InitializeProjectileArc() {
		projectileArcPoints = new GameObject[amount];
		for (int i = 0; i < projectileArcPoints.Length; i++) {
			GameObject g = Instantiate(projectileArcPointPrefab, new Vector2(0f, 0f), Quaternion.identity, gameObject.transform);
			projectileArcPoints[i] = g;
			projectileArcPoints[i].SetActive(false);
		}
	}

	public void UpdateProjectileArc(Rigidbody2D rigidbody, Vector2 position, float angle, float velocity) {
		EnableArc();

		float currentSpacing = spacing;

		float gravity = Physics2D.gravity.magnitude;
		//float velocity = (ballistaController.currentDrawLength / ballistaController.maxDrawLength) * ballista.maxShootForce;

		// if (ballista.nextProjectile != null) {
		// 	Rigidbody2D rb = ballista.nextProjectile.GetComponent<Rigidbody2D>();
		gravity *= rigidbody.gravityScale;
		velocity /= rigidbody.mass;
		//}

		//	float angle = ballista.flightGroove.eulerAngles.z;
		float radians = angle * Mathf.Deg2Rad;
		foreach (GameObject p in projectileArcPoints) {
			if (velocity <= 0f) {
				return;
			}
			float time = currentSpacing / velocity;
			float x = velocity * time * Mathf.Cos(radians);

			float y = (velocity * time * Mathf.Sin(radians)) - ((gravity * time * time) / 2f);

			p.transform.position = new Vector2(x, y) + position;
			currentSpacing += spacing;
		}
	}

	void EnableArc() {
		if (projectileArcPoints == null) {
			InitializeProjectileArc();
		}

		if (!projectileArcPoints[0].activeInHierarchy) {
			foreach (GameObject g in projectileArcPoints) {
				g.SetActive(true);
			}
		} else {
			return;
		}
	}

	public void DisableArc() {
		foreach (GameObject g in projectileArcPoints) {
			g.SetActive(false);
		}

	}
}