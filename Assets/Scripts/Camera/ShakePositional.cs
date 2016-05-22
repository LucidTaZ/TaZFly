using UnityEngine;

public class ShakePositional : MonoBehaviour {
	public static void ShakeAtLocation(Vector3 location, float magnitude, float roughness, float fadeInTime, float fadeOutTime) {
		float distance = (EZCameraShake.CameraShaker.Instance.GetComponent<Camera>().transform.position - location).magnitude;
		float adjustedMagnitude = Mathf.Clamp(magnitude * 100f / distance / distance, 0f, 3f);
		EZCameraShake.CameraShaker.Instance.ShakeOnce(adjustedMagnitude, roughness, fadeInTime, fadeOutTime);
	}
}
