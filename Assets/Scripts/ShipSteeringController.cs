using UnityEngine;

public class ShipSteeringController : MonoBehaviour {

	public float HorizontalMoveRate;
	public float VerticalMoveRate;

	public float SteerInterpolationTime = 0.3f; // Rougly: Number of seconds to interpolate to a new vector

	protected void Steer (float dx, float dy) {
		float increment = Time.deltaTime / SteerInterpolationTime;
		
		Vector3 desiredVector = new Vector3(
			Mathf.Clamp(dx, -1.0f, 1.0f) * HorizontalMoveRate,
			Mathf.Clamp(dy, -1.0f, 1.0f) * VerticalMoveRate,
			GetComponent<Rigidbody>().velocity.z
			);
		
		GetComponent<Rigidbody>().velocity = Vector3.Lerp(
			GetComponent<Rigidbody>().velocity,
			desiredVector,
			increment
			);
		//Debug.Log("Steered to " + rigidbody.velocity + ".");
	}

	protected void Steer (Vector3 direction) {
		Steer(direction.x, direction.y);
	}

}
