using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BankByVelocity : MonoBehaviour {
	public float BankElevator;
	public float BankAileron;
	public float BankRudder;
	public float BankConvergeSpeed;

	Rigidbody thisRigidbody;

	Quaternion flightDirection;
	Quaternion flightDirectionInversed;

	void Awake () {
		thisRigidbody = GetComponent<Rigidbody>();
		flightDirection = transform.rotation;
		flightDirectionInversed = Quaternion.Inverse(flightDirection);
	}

	void Update () {
		// Important: When implementing this in FixedUpdate, the orientation does not properly zero out anymore after having had a collision, nasty bug.
		bankPlane(thisRigidbody.velocity);
	}

	void bankPlane (Vector3 velocity) {
		// Bank the plane based on current velocity:
		Quaternion targetRotation = Quaternion.identity;

		Vector3 velocityInForwardSpace = flightDirectionInversed * velocity;

		// TODO: Compute x and y based on the general direction of movement, instead of depending it on the initial flight direction?
		targetRotation *= Quaternion.Euler(-velocityInForwardSpace.y * BankElevator, 0.0f, 0.0f);
		targetRotation *= Quaternion.AngleAxis(velocityInForwardSpace.x * BankAileron, Vector3.up);
		targetRotation *= Quaternion.AngleAxis(-velocityInForwardSpace.x * BankRudder, Vector3.forward);

		//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, BankConvergeSpeed * Time.deltaTime);
		transform.rotation = targetRotation * flightDirection;
	}
}
