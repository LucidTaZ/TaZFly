using UnityEngine;

[RequireComponent(typeof(Hitpoints))]
public class AdjustExhaustByDamage : MonoBehaviour {
	public Exhaust ExhaustComponent;

	Hitpoints hitpoints;

	void Awake () {
		hitpoints = GetComponent<Hitpoints>();
	}

	void FixedUpdate () {
		adjustExhaust();
	}

	void adjustExhaust () {
		float damage = hitpoints.GetDamage(); // 0 to 1
		ExhaustComponent.SetSeverity(damage);
	}
}
