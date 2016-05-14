using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public GameObject Subject;
	private bool loaded = false;

	private Vector3 offset;

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
		if (loaded) {
			offset = transform.position - Subject.transform.position;
		} else {
			offset = transform.position;
		}
	}
	
	void LateUpdate () {
		if (loaded && Subject) {
			transform.position = Subject.transform.position + offset;
		}
	}
}
