using UnityEngine;

[System.Serializable]
public class Boundary {
	public float MinX, MinY, MaxX, MaxY;
}

public class ShipController : MonoBehaviour {

	protected Boundary FieldBoundary;

	public ParticleSystem Exhaust;

	public void Load (Boundary boundary) {
		FieldBoundary = boundary;
		if (GetComponent<BankByVelocity>()) {
			GetComponent<BankByVelocity>().Load(boundary);
		}
	}

	void FixedUpdate () {
		transform.position = clampPosition(transform.position, FieldBoundary);

		adjustExhaust();
	}

	static Vector3 clampPosition (Vector3 position, Boundary fieldBoundary) {
		return new Vector3(
			Mathf.Clamp(position.x, fieldBoundary.MinX, fieldBoundary.MaxX),
			Mathf.Clamp(position.y, fieldBoundary.MinY, fieldBoundary.MaxY),
			position.z
		);
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
