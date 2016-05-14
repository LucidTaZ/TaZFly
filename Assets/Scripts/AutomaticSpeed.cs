using UnityEngine;

// TODO: Take into account the initial rotation, and make sure the subject always travels in that direction, not simply the Z direction always

public class AutomaticSpeed : MonoBehaviour {

	public float MinSpeed;
	public float MaxSpeed;
	public float SlowHeight; // Height at which MinSpeed is achieved
	public float SpeedyHeight; // Height at which MaxSpeed is achieved
	
	void FixedUpdate () {
		float speed = computeSpeed(transform.position.y);
		GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, speed);
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
