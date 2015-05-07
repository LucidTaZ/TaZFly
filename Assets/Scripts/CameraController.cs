using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

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
		offset = transform.position; // Take over the relative position modeled in the editor.
	}
	
	void LateUpdate () {
		if (loaded) {
			transform.position = Subject.transform.position + offset;
		}
	}
}
