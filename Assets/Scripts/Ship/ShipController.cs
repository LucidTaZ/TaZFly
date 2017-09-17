using UnityEngine;

[System.Serializable]
public class Boundary {
	public float MinX, MinY, MaxX, MaxY;
}

public class ShipController : MonoBehaviour {

	public Exhaust ExhaustComponent;

	public void Load (Boundary boundary) {
		if (GetComponent<BankByVelocity>()) {
			GetComponent<BankByVelocity>().Load(boundary);
		}
	}

	void FixedUpdate () {
		adjustExhaust();
	}

	void adjustExhaust () {
		if (GetComponent<Hitpoints>()) {
			float damage = GetComponent<Hitpoints>().GetDamage(); // 0 to 1
			ExhaustComponent.SetSeverity(damage);
		} else {
			Debug.LogWarning("Trying to adjust exhaust without expected component.");
		}
	}
}
