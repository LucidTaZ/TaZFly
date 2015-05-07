using UnityEngine;
using System.Collections;

public class BaseAI : ShipSteeringController {

	protected void AvoidPoint (Vector3 point) {
		Vector3 dangerDirection = (point - transform.position).normalized;
		Vector3 resolutionDirection = Quaternion.Euler(0.0f, 0.0f, 180.0f) * dangerDirection;

		resolutionDirection.x = Mathf.Sign(resolutionDirection.x);
		resolutionDirection.y = Mathf.Sign(resolutionDirection.y);

		//Debug.Log("Avoiding by going " + resolutionDirection + ".");
		Steer(resolutionDirection);
	}

	protected void FlyLower () {
		//Debug.Log("Lowering.");
		Steer(0.0f, -2.0f);
	}

	protected void FlyStraight () {
		//Debug.Log("Flying straight.");
		Steer(0.0f, 0.0f);
	}

}
