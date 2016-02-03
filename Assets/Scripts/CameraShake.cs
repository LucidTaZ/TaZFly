using UnityEngine;

public class CameraShake : MonoBehaviour {

	public float ConvergeSpeed = 1f;
	public float Diminish = 0.25f;

	float shakeTimeLeft = 0f;
	float currentMagnitude = 0f;

	void Update () {
		if (shakeTimeLeft > 0) {
			updateShake();
			shakeTimeLeft -= Time.deltaTime;
			currentMagnitude *= Diminish * Time.deltaTime;
		}
	}

	public void Shake (float duration = 0.5f, float magnitude = 1f) {
		if (shakeTimeLeft > 0) {
			if (magnitude < currentMagnitude) {
				return;
			}
		}
		shakeTimeLeft = duration;
		currentMagnitude = magnitude;
	}

	public void ShakeAtLocation(Vector3 location, float duration = 0.5f, float magnitude = 1f) {
		float distance = (transform.position - location).magnitude;
		float adjustedMagnitude = magnitude * 100f / distance / distance;
		Shake(duration, adjustedMagnitude);
	}

	void updateShake () {
		transform.rotation *= Quaternion.Slerp(Quaternion.identity, Random.rotation, currentMagnitude * ConvergeSpeed * Time.deltaTime);
	}
}
