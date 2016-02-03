using UnityEngine;

public class CameraShakePositional : MonoBehaviour {
	public static void ShakeAtLocation(Vector3 location, float magnitude, float roughness, float fadeInTime, float fadeOutTime) {
		float distance = (EZCameraShake.CameraShaker.Instance.GetComponent<Camera>().transform.position - location).magnitude;
		float adjustedMagnitude = magnitude * 100f / distance / distance;
		EZCameraShake.CameraShaker.Instance.ShakeOnce(adjustedMagnitude, roughness, fadeInTime, fadeOutTime);
	}
}
