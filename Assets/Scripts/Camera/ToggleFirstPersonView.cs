using UnityEngine;

[RequireComponent(typeof(FollowSubject))]
public class ToggleFirstPersonView : MonoBehaviour, CameraAttachmentInterface {

	public GameObject Subject;
	bool loaded;

	FollowSubject followSubject;
	SwoonWithSubject swoonWithSubject;

	bool isCurrentlyFirstPerson = false;

	Quaternion originalRotation;

	void Awake () {
		followSubject = GetComponent<FollowSubject>();
		swoonWithSubject = GetComponent<SwoonWithSubject>();

		if (Subject != null) {
			loaded = true;
		}
	}

	void Start () {
		// Take over setting modeled in the editor
		originalRotation = transform.localRotation;
	}

	public void Load (GameObject subject) {
		Subject = subject;
		loaded = true;
	}

	public void Unload () {
		if (isCurrentlyFirstPerson) {
			SwitchToThirdPerson();
		}
		loaded = false;
	}

	void Update () {
		if (!loaded) {
			return;
		}

		if (!Input.GetButtonDown("ToggleFirstPerson")) {
			return;
		}

		if (isCurrentlyFirstPerson) {
			SwitchToThirdPerson();
		} else {
			SwitchToFirstPerson();
		}
	}

	public void SwitchToFirstPerson () {
		if (swoonWithSubject) {
			swoonWithSubject.enabled = false;
		}
		followSubject.enabled = false;
		transform.parent = Subject.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		isCurrentlyFirstPerson = true;
	}

	public void SwitchToThirdPerson () {
		if (swoonWithSubject) {
			swoonWithSubject.enabled = true;
		}
		transform.parent = null;
		transform.localRotation = originalRotation;
		followSubject.enabled = true;
		isCurrentlyFirstPerson = false;
	}
}
