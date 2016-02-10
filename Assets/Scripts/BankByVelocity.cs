using UnityEngine;

public class BankByVelocity : MonoBehaviour {
	protected Boundary FieldBoundary;

	public float BankElevator;
	public float BankAileron;
	public float BankRudder;
	public float BankConvergeSpeed;

	protected BoundaryEnforcer boundaryEnforcer;

	public void Start () {
		boundaryEnforcer = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoundaryEnforcer>();
	}

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

		if (!boundaryEnforcer || boundaryEnforcer.IsInVerticalBounds(transform.position)) {
			targetRotation *= Quaternion.Euler(-velocity.y * BankElevator, 0.0f, 0.0f);
		}
		if (!boundaryEnforcer || boundaryEnforcer.IsInHorizontalBounds(transform.position)) {
			targetRotation *= Quaternion.AngleAxis(velocity.x * BankAileron, Vector3.up);
			targetRotation *= Quaternion.AngleAxis(-velocity.x * BankRudder, Vector3.forward);
		}

		//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, BankConvergeSpeed * Time.deltaTime);
		transform.rotation = targetRotation;
	}
}
