using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CannonBall : EnemyProjectile {

    // Velocity limit as to when it should start destroying itself
    protected readonly float velocityDestroyLimit = 1f;
    // Time in seconds it takes to start destroying if under velocity limit
    protected readonly float timeToDestroy = 3f;
    private float currentTimeToDestroy;

    private bool shouldDestroy = false;
    // Time it takes to fade
    protected readonly float fadeTime = 2f;
    private float currentFadeTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Color currentColor;

    Rigidbody2D rb;
    private float currentVelocity;

    public enum CannonBallType {
        Standard,
        Explosive
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentColor = spriteRenderer.color;
    }

    void Update() {
        FadeToDestroy();

    }

    void FixedUpdate() {
        currentVelocity = rb.velocity.magnitude;
    }

    private void FadeToDestroy() {
        if (currentVelocity < velocityDestroyLimit) {
            currentTimeToDestroy += Time.deltaTime;
        } else {
            currentTimeToDestroy = 0f;
        }

        if (currentTimeToDestroy >= timeToDestroy) {
            shouldDestroy = true;
        }

        if (shouldDestroy) {
            FadeOut();
        }
    }

    private void FadeOut() {
        currentColor.a = Mathf.Lerp(1f, 0f, currentFadeTime);
        spriteRenderer.color = currentColor;
        currentFadeTime += Time.deltaTime/fadeTime;
        if (currentColor.a <= 0f) {
            Destroy(gameObject);
        }
    }

}