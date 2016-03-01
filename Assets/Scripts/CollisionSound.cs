using UnityEngine;
using System.Collections;

public class CollisionSound : MonoBehaviour {

	public AudioClip[] clips;

	public float Cooldown = 0.5f;

	public float MagThreshold = 2f; // Minimum impulse
	public float VelocityThreshold = 10f; // Minimum velocity

	float cooldownTimer = 0f;

	void Start () {
		if (clips.Length == 0) {
			throw new UnityException("No audio clips assigned.");
		}
	}

	void Update () {
		if (cooldownTimer > 0f) {
			cooldownTimer -= Time.deltaTime;
		}
	}

	void OnCollisionStay (Collision collisionInfo) {
		if (
			cooldownTimer <= 0f
			&& collisionInfo.impulse.sqrMagnitude > MagThreshold*MagThreshold
			&& GetComponent<Rigidbody>().velocity.sqrMagnitude > VelocityThreshold*VelocityThreshold
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
