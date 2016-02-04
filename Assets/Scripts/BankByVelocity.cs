using UnityEngine;

public class BankByVelocity : MonoBehaviour {
	protected Boundary FieldBoundary;

	protected const float EPSILON = 0.0001f;

	public float BankElevator;
	public float BankAileron;
	public float BankRudder;
	public float BankConvergeSpeed;

	public void Load (Boundary boundary) {
		FieldBoundary = boundary;
	}

	void FixedUpdate () {
		bankPlane(GetComponent<Rigidbody>().velocity);
	}

	void bankPlane (Vector3 velocity) {
		// Bank the plane based on current velocity:
		Quaternion targetRotation = Quaternion.identity;

		if (transform.position.y > FieldBoundary.MinY + EPSILON && transform.position.y < FieldBoundary.MaxY - EPSILON) {
			targetRotation *= Quaternion.Euler(-velocity.y * BankElevator, 0.0f, 0.0f);
		}

		if (transform.position.x > FieldBoundary.MinX + EPSILON && transform.position.x < FieldBoundary.MaxX - EPSILON) {
			targetRotation *= Quaternion.AngleAxis(velocity.x * BankAileron, Vector3.up);
			targetRotation *= Quaternion.AngleAxis(-velocity.x * BankRudder, Vector3.forward);
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, BankConvergeSpeed);
	}
}
