using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionSound : MonoBehaviour {
	public AudioClip[] clips;

	public float Cooldown = 0.5f;

	public float MagThreshold = 2f; // Minimum impulse
	public float VelocityThreshold = 10f; // Minimum velocity

	Rigidbody thisRigidbody;

	float cooldownTimer;

	void Awake () {
		thisRigidbody = GetComponent<Rigidbody>();
		if (clips.Length == 0) {
			throw new UnityException("No audio clips assigned.");
		}
	}

	void Update () {
		if (cooldownTimer > 0f) {
			cooldownTimer -= Time.deltaTime;
		}
	}

	void OnCollisionEnter (Collision collisionInfo) {
		tryToPlaySound(collisionInfo.impulse.sqrMagnitude);
	}

	void OnCollisionStay (Collision collisionInfo) {
		tryToPlaySound(collisionInfo.impulse.sqrMagnitude);
	}

	void tryToPlaySound (float sqrMagnitude) {
		if (
			cooldownTimer <= 0f
			&& sqrMagnitude > MagThreshold*MagThreshold
			&& thisRigidbody.velocity.sqrMagnitude > VelocityThreshold*VelocityThreshold
		) {
			playSound();
			cooldownTimer = Cooldown;
		}
	}

	void playSound () {
		AudioClip clip = clips[Random.Range(0, clips.Length)];
		AudioSource.PlayClipAtPoint(clip, transform.position);
	}
}
