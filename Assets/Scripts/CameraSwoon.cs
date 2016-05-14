using UnityEngine;

public class CameraSwoon : MonoBehaviour {

	public float MaxDeviation = 0.2f;
	public float ConvergeSpeed = 0.6f;

	public GameObject Subject;
	private bool loaded = false;

	private Quaternion rotation;

	void Awake () {
		if (Subject != null) {
			loaded = true;
		}
	}

	public void Load (GameObject subject) {
		Subject = subject;
		loaded = true;
	}

	public void Unload () {
		loaded = false;
	}

	void Start () {
		// Take over the relative settings modeled in the editor.
		rotation = transform.rotation;
	}
	
	void LateUpdate () {
		if (loaded && Subject) {
			Quaternion maximumDeviation = Quaternion.Slerp(rotation, Subject.transform.rotation, MaxDeviation);
			transform.rotation = Quaternion.Slerp(transform.rotation, maximumDeviation, ConvergeSpeed * Time.deltaTime);
		}
	}
}
