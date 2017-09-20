using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutomaticSpeed : MonoBehaviour {
	public float MinSpeed;
	public float MaxSpeed;
	public float SlowHeight; // Height at which MinSpeed is achieved
	public float SpeedyHeight; // Height at which MaxSpeed is achieved
	public float BoundaryLeft; // x coordinate at which speed starts to drop
	public float BoundaryRight; // x coordinate at which speed starts to drop

	public AnimationCurve RelativeSpeedByRelativeAltitude;
	public AnimationCurve OutOfBoundaryPenaltyByDeviation;

	Rigidbody thisRigidbody;

	Quaternion flightDirection;
	Quaternion flightDirectionInversed;

	void Awake () {
		thisRigidbody = GetComponent<Rigidbody>();
		flightDirection = transform.rotation;
		flightDirectionInversed = Quaternion.Inverse(flightDirection);
	}

	void FixedUpdate () {
		float speed = computeSpeed(transform.position.y) * (1.0f - computeOutOfBoundaryPenalty(transform.position.x));
		Vector3 velocityInForwardSpace = flightDirectionInversed * thisRigidbody.velocity;
		velocityInForwardSpace.z = speed;
		thisRigidbody.velocity = flightDirection * velocityInForwardSpace;
	}
	
	float computeSpeed (float altitude) {
		altitude = Mathf.Clamp(altitude, SpeedyHeight, SlowHeight);
		float relativeAltitude = (altitude - SpeedyHeight) / (SlowHeight - SpeedyHeight);
		float relativeSpeed = RelativeSpeedByRelativeAltitude.Evaluate(relativeAltitude);
		return MinSpeed + relativeSpeed * (MaxSpeed - MinSpeed);
	}

	// Returns 0..1, 0 is no penalty
	float computeOutOfBoundaryPenalty (float positionX) {
		if (positionX < BoundaryLeft) {
			return OutOfBoundaryPenaltyByDeviation.Evaluate(BoundaryLeft - positionX);
		} else if (BoundaryRight < positionX) {
			return OutOfBoundaryPenaltyByDeviation.Evaluate(positionX - BoundaryRight);
		}
		return 0.0f;
	}

	float getSpeed () {
		return thisRigidbody.velocity.z;
	}

	public float GetRelativeSpeed () {
		return (getSpeed() - MinSpeed) / (MaxSpeed - MinSpeed);
	}
}
