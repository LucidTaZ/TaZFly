using UnityEngine;

public class AvoidingAI : BaseAI {

	public float LookAheadDistance;
	public float LookDownAheadDistance;
	RaycastHit hitInfo;

	float lookDownAheadDistanceStart;

	protected override void Awake () {
		base.Awake();
		lookDownAheadDistanceStart = LookDownAheadDistance;
	}

	void Update () {
		bool sweepResultForward = GetComponent<Rigidbody>().SweepTest(Vector3.forward, out hitInfo, LookAheadDistance);

		if (sweepResultForward) {
			AvoidPoint(hitInfo.point);
		} else {
			bool sweepResultDownAhead = GetComponent<Rigidbody>().SweepTest(Vector3.forward + Vector3.down * 0.5f, out hitInfo, LookDownAheadDistance);
			if (sweepResultDownAhead) {
				if (hitInfo.distance < 0.5f * LookDownAheadDistance) {
					FlyStraight();
				} else {
					// Continue flying in the same direction
				}
			} else {
				FlyLower();
			}
		}
	}

	void LateUpdate () {
		// The more damage, the more cautious it becomes
		Hitpoints hitpoints = GetComponent<Hitpoints>();
		float damage = hitpoints.GetDamage();
		LookDownAheadDistance = lookDownAheadDistanceStart * (1 + 2 * damage);
	}

}
