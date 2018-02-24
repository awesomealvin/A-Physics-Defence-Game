﻿using System.Collections;
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

	[SerializeField]
	Ballista ballista;
	[SerializeField]
	BallistaController ballistaController;

	void Awake() {
		amount = Mathf.RoundToInt(length / spacing);
		projectileArcPoints = new GameObject[amount];
		for (int i = 0; i < projectileArcPoints.Length; i++) {
			GameObject g = Instantiate(projectileArcPointPrefab, ballista.shootPoint.position, Quaternion.identity, gameObject.transform);
			projectileArcPoints[i] = g;
		}
	}

	void Update() {
		if (ballistaController.isMouseDown) {
			UpdateProjectileArc();
		}
	}

	public void UpdateProjectileArc() {
		float currentSpacing = spacing;

		float gravity = Physics2D.gravity.magnitude;
		float velocity = (ballistaController.currentDrawLength / ballistaController.maxDrawLength) * ballista.maxShootForce;

		if (ballista.nextProjectile != null) {
			Rigidbody2D rb = ballista.nextProjectile.GetComponent<Rigidbody2D>();
			gravity *= rb.gravityScale;
			velocity /= rb.mass;
		}

		float angle = ballista.flightGroove.eulerAngles.z;
		float radians = angle * Mathf.Deg2Rad;
		foreach (GameObject p in projectileArcPoints) {
			if (velocity <= 0f) {
				return;
			}
			float time = currentSpacing / velocity;
			float x = velocity * time * Mathf.Cos(radians);

			float y = (velocity * time * Mathf.Sin(radians)) - ((gravity * time * time) / 2f);

			p.transform.position = new Vector3(x, y, 0f) + ballista.shootPoint.position;
			currentSpacing += spacing;
		}
	}
}