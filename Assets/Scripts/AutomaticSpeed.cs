using UnityEngine;

// TODO: Take into account the initial rotation, and make sure the subject always travels in that direction, not simply the Z direction always

public class AutomaticSpeed : MonoBehaviour {

	public float MinSpeed;
	public float MaxSpeed;
	public float SlowHeight; // Height at which MinSpeed is achieved
	public float SpeedyHeight; // Height at which MaxSpeed is achieved

	Quaternion flightDirection;
	Quaternion flightDirectionInversed;

	void Start () {
		flightDirection = transform.rotation;
		flightDirectionInversed = Quaternion.Inverse(flightDirection);
	}

	void FixedUpdate () {
		float speed = computeSpeed(transform.position.y);
		Vector3 velocityInForwardSpace = flightDirectionInversed * GetComponent<Rigidbody>().velocity;
		velocityInForwardSpace.z = speed;
		GetComponent<Rigidbody>().velocity = flightDirection * velocityInForwardSpace;
	}
	
	float computeSpeed (float height) {
		// TODO: Experiment a bit with nonlinear speed relation. Quadratic maybe? We really want flying low to be rewarding but also dangerous!
		height = Mathf.Clamp(height, SpeedyHeight, SlowHeight);
		float perc = (height - SlowHeight) / (SpeedyHeight - SlowHeight);
		return MinSpeed + perc * (MaxSpeed - MinSpeed);
	}

	float getSpeed () {
		return GetComponent<Rigidbody>().velocity.z;
	}

	public float GetRelativeSpeed () {
		return (getSpeed() - MinSpeed) / (MaxSpeed - MinSpeed);
	}
}
