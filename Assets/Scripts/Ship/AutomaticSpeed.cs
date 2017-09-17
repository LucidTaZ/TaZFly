using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutomaticSpeed : MonoBehaviour {
	public float MinSpeed;
	public float MaxSpeed;
	public float SlowHeight; // Height at which MinSpeed is achieved
	public float SpeedyHeight; // Height at which MaxSpeed is achieved

	Rigidbody thisRigidbody;

	Quaternion flightDirection;
	Quaternion flightDirectionInversed;

	void Awake () {
		thisRigidbody = GetComponent<Rigidbody>();
		flightDirection = transform.rotation;
		flightDirectionInversed = Quaternion.Inverse(flightDirection);
	}

	void FixedUpdate () {
		float speed = computeSpeed(transform.position.y);
		Vector3 velocityInForwardSpace = flightDirectionInversed * thisRigidbody.velocity;
		velocityInForwardSpace.z = speed;
		thisRigidbody.velocity = flightDirection * velocityInForwardSpace;
	}
	
	float computeSpeed (float height) {
		// TODO: Experiment a bit with nonlinear speed relation. Quadratic maybe? We really want flying low to be rewarding but also dangerous!
		height = Mathf.Clamp(height, SpeedyHeight, SlowHeight);
		float perc = (height - SlowHeight) / (SpeedyHeight - SlowHeight);
		return MinSpeed + perc * (MaxSpeed - MinSpeed);
	}

	float getSpeed () {
		return thisRigidbody.velocity.z;
	}

	public float GetRelativeSpeed () {
		return (getSpeed() - MinSpeed) / (MaxSpeed - MinSpeed);
	}
}
