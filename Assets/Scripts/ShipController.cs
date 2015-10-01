using UnityEngine;

[System.Serializable]
public class Boundary {
	public float MinX, MinY, MaxX, MaxY;
}

public class ShipController : MonoBehaviour {

	protected int StartHitpoints;
	public int Hitpoints;

	public float MinSpeed;
	public float MaxSpeed;
	public float SlowHeight; // Height at which MinSpeed is achieved
	public float SpeedyHeight; // Height at which MaxSpeed is achieved

	protected Boundary FieldBoundary;

	public float BankElevator;
	public float BankAileron;
	public float BankRudder;
	public float BankConvergeSpeed;

	public Animator PropellerAnimator;
	public ParticleSystem Exhaust;

	protected const float EPSILON = 0.0001f;

	public void Load (Boundary boundary) {
		FieldBoundary = boundary;
	}

	void Start () {
		StartHitpoints = Hitpoints; // Take over setting from editor.
	}

	void FixedUpdate () {
		float speed = computeSpeed(transform.position.y);
		float speedPerc = (speed - MinSpeed) / (MaxSpeed - MinSpeed);

		GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, speed);

		bankPlane(GetComponent<Rigidbody>().velocity);

		transform.position = clampPosition(transform.position, FieldBoundary);

		animatePropeller(speedPerc);

		adjustExhaust();
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.collider.GetComponents<CollisionExplosion>().Length == 0) {
			// Is otherwise already handled in its CollisionExplosion script
			DecreaseHitpoints((int)collision.relativeVelocity.magnitude);
		}
	}
	
	public void DecreaseHitpoints (int delta = 1) {
		//Debug.Log("Taking " + delta + " damage.");
		Hitpoints -= delta;

		if (!IsAlive() && GetComponents<PlayerController>().Length == 0) {
			// AI player died
			Debug.Log("AI Player died.");
			Destroy(gameObject);
		}
	}

	public float GetRelativeHitpoints () {
		return (float)Hitpoints / StartHitpoints;
	}

	public bool IsAlive () {
		return Hitpoints > 0;
	}

	float computeSpeed (float height) {
		// TODO: Experiment a bit with nonlinear speed relation. Quadratic maybe? We really want flying low to be rewarding but also dangerous!
		height = Mathf.Clamp(height, SpeedyHeight, SlowHeight);
		float perc = (height - SlowHeight) / (SpeedyHeight - SlowHeight);
		return MinSpeed + perc * (MaxSpeed - MinSpeed);
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

	static Vector3 clampPosition (Vector3 position, Boundary fieldBoundary) {
		return new Vector3(
			Mathf.Clamp(position.x, fieldBoundary.MinX, fieldBoundary.MaxX),
			Mathf.Clamp(position.y, fieldBoundary.MinY, fieldBoundary.MaxY),
			position.z
		);
	}

	void animatePropeller (float speedPerc) {
		// Control propeller animation speed:
		PropellerAnimator.SetFloat("Speed", speedPerc); // This Animator wants a value from 0 to 1.
	}

	void adjustExhaust () {
		float damage = Mathf.Clamp (1.0f - GetRelativeHitpoints(), 0.0f, 1.0f);
		Exhaust.emissionRate = damage * 10;
		Exhaust.startLifetime = damage * 4.5f + 0.5f;
		Exhaust.startColor = Color.Lerp(Color.white, Color.black, damage);
	}
}
