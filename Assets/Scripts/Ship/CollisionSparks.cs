using UnityEngine;
using System.Collections;

public class CollisionSparks : MonoBehaviour {

	public float MagThreshold = 1f; // Minimum impulse
	public float VelocityThreshold = 5f; // Minimum velocity

	public ParticleSystem Particles;

	void OnCollisionStay (Collision collisionInfo) {
		if (
			!Particles.isPlaying
			&& collisionInfo.impulse.sqrMagnitude > MagThreshold*MagThreshold
			&& GetComponent<Rigidbody>().velocity.sqrMagnitude > VelocityThreshold*VelocityThreshold
		) {
			makeSparks();
		}
	}

	void makeSparks () {
		Particles.Play();
	}
}
