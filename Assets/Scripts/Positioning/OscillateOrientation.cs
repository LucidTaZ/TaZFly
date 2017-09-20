using UnityEngine;

public class OscillateOrientation : MonoBehaviour {
	public float Period = 1.0f;
	public float Amplitude = 360.0f;
	public Vector3 Axis = Vector3.up;

	public bool RandomizePhaseOffset;
	public float PhaseOffset;

	void Awake () {
		if (RandomizePhaseOffset) {
			PhaseOffset = Random.Range(0, Period);
		}
	}

	void Update () {
		float angle = Amplitude * Mathf.Sin(2 * Mathf.PI * (Time.time + PhaseOffset) / Period);
		transform.localRotation = Quaternion.AngleAxis(angle, Axis);
	}
}
