using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Hitpoints))]
public class AvoidingAI : BaseAI {

	public float LookAheadDistance;
	public float LookDownAheadDistance;
	RaycastHit hitInfo;

	Rigidbody thisRigigbody;
	Hitpoints hitpoints;

	float lookDownAheadDistanceStart;

	protected override void Awake () {
		base.Awake();
		thisRigigbody = GetComponent<Rigidbody>();
		hitpoints = GetComponent<Hitpoints>();
		lookDownAheadDistanceStart = LookDownAheadDistance;
	}

	void Update () {
		bool sweepResultForward = thisRigigbody.SweepTest(Vector3.forward, out hitInfo, LookAheadDistance);

		if (sweepResultForward) {
			AvoidPoint(hitInfo.point);
		} else {
			bool sweepResultDownAhead = thisRigigbody.SweepTest(Vector3.forward + Vector3.down * 0.5f, out hitInfo, LookDownAheadDistance);
			if (sweepResultDownAhead) {
				if (hitInfo.distance < 0.5f * LookDownAheadDistance) {
					FlyStraight();
				} else {
					// Converge slowly to center (to avoid boundary penalty)
					SteerLocalSpace(-0.05f * Mathf.Clamp(transform.position.x, -5.0f, 5.0f), 0.0f);
				}
			} else {
				FlyLower();
			}
		}
	}

	void LateUpdate () {
		// The more damage, the more cautious it becomes
		float damage = hitpoints.GetDamage();
		LookDownAheadDistance = lookDownAheadDistanceStart * (1 + 2 * damage);
	}

}
