using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionSparks : MonoBehaviour {
	public float MagThreshold = 1f; // Minimum impulse
	public float VelocityThreshold = 5f; // Minimum velocity

	public ParticleSystem Particles;

	Rigidbody thisRigidbody;

	void Awake () {
		thisRigidbody = GetComponent<Rigidbody>();
	}

	void OnCollisionStay (Collision collisionInfo) {
		if (
			!Particles.isPlaying
			&& collisionInfo.impulse.sqrMagnitude > MagThreshold*MagThreshold
			&& thisRigidbody.velocity.sqrMagnitude > VelocityThreshold*VelocityThreshold
		) {
			makeSparks();
		}
	}

	void makeSparks () {
		Particles.Play();
	}
}
