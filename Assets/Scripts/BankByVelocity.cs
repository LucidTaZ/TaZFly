using UnityEngine;

public class BankByVelocity : MonoBehaviour {
	protected Boundary FieldBoundary;

	public float BankElevator;
	public float BankAileron;
	public float BankRudder;
	public float BankConvergeSpeed;

	public void Load (Boundary boundary) {
		FieldBoundary = boundary;
	}

	void Update () {
		// Important: When implementing this in FixedUpdate, the orientation does not properly zero out anymore after having had a collision, nasty bug.
		bankPlane(GetComponent<Rigidbody>().velocity);
	}

	void bankPlane (Vector3 velocity) {
		// Bank the plane based on current velocity:
		Quaternion targetRotation = Quaternion.identity;

		if (transform.position.y > FieldBoundary.MinY && transform.position.y < FieldBoundary.MaxY) {
			targetRotation *= Quaternion.Euler(-velocity.y * BankElevator, 0.0f, 0.0f);
		}

		if (transform.position.x > FieldBoundary.MinX && transform.position.x < FieldBoundary.MaxX) {
			targetRotation *= Quaternion.AngleAxis(velocity.x * BankAileron, Vector3.up);
			targetRotation *= Quaternion.AngleAxis(-velocity.x * BankRudder, Vector3.forward);
		}

		//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, BankConvergeSpeed * Time.deltaTime);
		transform.rotation = targetRotation;
	}
}
