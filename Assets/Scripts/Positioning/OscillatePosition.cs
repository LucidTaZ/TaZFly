using UnityEngine;

public class OscillatePosition : MonoBehaviour {
	public float Period = 1.0f;
	public float Amplitude = 1.0f;
	public Vector3 Direction = Vector3.up;

	public bool RandomizePhaseOffset;
	public float PhaseOffset;

	Vector3 maxExtent;

	void Awake () {
		maxExtent = Direction.normalized * Amplitude;
		if (RandomizePhaseOffset) {
			PhaseOffset = Random.Range(0, Period);
		}
	}

	void Update () {
		transform.localPosition = maxExtent * Mathf.Sin(2 * Mathf.PI * (Time.time + PhaseOffset) / Period);
	}
}
