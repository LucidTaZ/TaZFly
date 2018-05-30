using UnityEngine;

public class SwoonWithSubject : MonoBehaviour, CameraAttachmentInterface {

	public float MaxDeviation = 0.2f;
	public float ConvergeSpeed = 0.6f;

	public GameObject Subject;
	bool loaded;

	Quaternion rotation;

	void Awake () {
		if (Subject != null) {
			loaded = true;
		}
		// Take over the relative settings modeled in the editor.
		rotation = transform.rotation;
	}

	public void Load (GameObject subject) {
		Subject = subject;
		loaded = true;
	}

	public void Unload () {
		loaded = false;
	}

	void LateUpdate () {
		if (loaded && Subject) {
			Quaternion maximumDeviation = Quaternion.Slerp(rotation, Subject.transform.rotation, MaxDeviation);
			transform.rotation = Quaternion.Slerp(transform.rotation, maximumDeviation, ConvergeSpeed * Time.deltaTime);
		}
	}
}
