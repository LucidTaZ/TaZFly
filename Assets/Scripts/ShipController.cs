using UnityEngine;

[System.Serializable]
public class Boundary {
	public float MinX, MinY, MaxX, MaxY;
}

public class ShipController : MonoBehaviour {

	protected Boundary FieldBoundary;

	public float BankElevator;
	public float BankAileron;
	public float BankRudder;
	public float BankConvergeSpeed;

	public Animator PropellerAnimator;
	public ParticleSystem Exhaust;

	protected const float EPSILON = 0.0001f;

	public void Load (Boundary boundary) {
		Exhaust.gameObject.SetActive(false); // Fix for emission bug in 5.3.1f1. Should be fixed in 5.3.1p1 or 5.3.2
		FieldBoundary = boundary;
	}

	void FixedUpdate () {
		bankPlane(GetComponent<Rigidbody>().velocity);

		transform.position = clampPosition(transform.position, FieldBoundary);

		animatePropeller();

		adjustExhaust();
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

	void animatePropeller () {
		// Control propeller animation speed:
		if (GetComponent<AutomaticSpeed>()) {
			PropellerAnimator.SetFloat("Speed", GetComponent<AutomaticSpeed>().GetRelativeSpeed()); // This Animator wants a value from 0 to 1.
		} else {
			Debug.LogWarning("Trying to animate propeller without expected component.");
			PropellerAnimator.SetFloat("Speed", 1);
		}
	}

	void adjustExhaust () {
		if (GetComponent<Hitpoints>()) {
			float damage = GetComponent<Hitpoints>().GetDamage(); // 0 to 1
			ParticleSystem.MinMaxCurve emissionRate = new ParticleSystem.MinMaxCurve(damage * 10);
			ParticleSystem.EmissionModule em = Exhaust.emission;
			em.rate = emissionRate;
			Exhaust.startLifetime = damage * 4.5f + 0.5f;
			Exhaust.startColor = Color.Lerp(Color.white, Color.black, damage);
		} else {
			Debug.LogWarning("Trying to adjust exhaust without expected component.");
		}
	}
}
