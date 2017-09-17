using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipSteeringController : MonoBehaviour {
	public float HorizontalMoveRate;
	public float VerticalMoveRate;

	public float SteerInterpolationTime = 0.3f; // Rougly: Number of seconds to interpolate to a new vector

	Rigidbody thisRigidbody;

	Quaternion flightDirection;
	Quaternion flightDirectionInversed;

	protected virtual void Awake () {
		// protected virtual to enable this class being extended (leaving it private (the default) causes Start() to
		// never be called, even if the child class does not implement it explicitly.)
		thisRigidbody = GetComponent<Rigidbody>();

		flightDirection = transform.rotation;
		flightDirectionInversed = Quaternion.Inverse(flightDirection);
		//Debug.Log("Flight direction: " + flightDirection);
	}

	protected void SteerLocalSpace (float dx, float dy) {
		float increment = Time.deltaTime / SteerInterpolationTime;

		Vector3 velocityInForwardSpace = flightDirectionInversed * thisRigidbody.velocity;

		Vector3 desiredVectorInForwardSpace = new Vector3(
			Mathf.Clamp(dx, -1.0f, 1.0f) * HorizontalMoveRate,
			Mathf.Clamp(dy, -1.0f, 1.0f) * VerticalMoveRate,
			velocityInForwardSpace.z
		);
		
		thisRigidbody.velocity = flightDirection * Vector3.Lerp(
			velocityInForwardSpace,
			desiredVectorInForwardSpace,
			increment
		);
	}

	protected void SteerWorldSpace (Vector3 direction) {
		Vector3 directionInForwardSpace = flightDirectionInversed * direction;
		SteerLocalSpace(directionInForwardSpace.x, directionInForwardSpace.y);
	}

}
