using UnityEngine;

public class BaseAI : ShipSteeringController {

	protected void AvoidPoint (Vector3 point) {
		Vector3 dangerDirection = (point - transform.position).normalized;
		Vector3 resolutionDirection = Quaternion.Euler(0.0f, 0.0f, 180.0f) * dangerDirection;

		resolutionDirection.x = Mathf.Sign(resolutionDirection.x);
		resolutionDirection.y = Mathf.Sign(resolutionDirection.y);

		Steer(resolutionDirection);
	}

	protected void FlyLower () {
		Steer(0.0f, -2.0f);
	}

	protected void FlyStraight () {
		Steer(0.0f, 0.0f);
	}

}
