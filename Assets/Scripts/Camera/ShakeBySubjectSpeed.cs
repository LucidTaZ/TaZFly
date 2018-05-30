using UnityEngine;
using EZCameraShake;

[RequireComponent(typeof(CameraShaker))]
public class ShakeBySubjectSpeed : MonoBehaviour, CameraAttachmentInterface {
	public GameObject Subject;
	Rigidbody subjectBody;
	bool loaded;

	public AnimationCurve FrequencyBySpeed;
	public AnimationCurve MagnitudeBySpeed;

	CameraShaker shaker;
	CameraShakeInstance shake;

	void Awake () {
		shaker = GetComponent<CameraShaker>();

		if (Subject != null) {
			subjectBody = Subject.GetComponentInChildren<Rigidbody>();
			loaded = true;
		}
	}

	public void Load (GameObject subject) {
		Subject = subject;
		subjectBody = Subject.GetComponentInChildren<Rigidbody>();
		loaded = true;
	}

	public void Unload () {
		Subject = null;
		subjectBody = null;
		loaded = false;
	}

	public void Start () {
		shake = shaker.StartShake(1, 1, 0);
		shake.ScaleRoughness = 0;
		shake.ScaleMagnitude = 0;
	}

	public void Update () {
		if (loaded) {
			float speed = subjectBody.velocity.z;
			shake.ScaleRoughness = FrequencyBySpeed.Evaluate(speed);
			shake.ScaleMagnitude = MagnitudeBySpeed.Evaluate(speed);
		}
	}
}
