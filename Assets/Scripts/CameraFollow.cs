using UnityEngine;

public class CameraFollow : MonoBehaviour {

	private GameObject Subject;
	private bool loaded = false;

	private Vector3 offset;

	public void Load (GameObject subject) {
		Subject = subject;
		loaded = true;
	}

	public void Unload () {
		loaded = false;
	}

	void Start () {
		// Take over the relative settings modeled in the editor.
		offset = transform.position;
	}
	
	void LateUpdate () {
		if (loaded) {
			transform.position = Subject.transform.position + offset;
		}
	}
}
