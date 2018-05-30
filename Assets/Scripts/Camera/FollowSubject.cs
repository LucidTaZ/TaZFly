using UnityEngine;

public class FollowSubject : MonoBehaviour, CameraAttachmentInterface {

	public GameObject Subject;
	bool loaded;

	public Vector3 Offset;

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
			Offset = transform.position - Subject.transform.position;
		} else {
			Offset = transform.position;
		}
	}
	
	void LateUpdate () {
		if (loaded && Subject) {
			transform.position = Subject.transform.position + Offset;
		}
	}
}
