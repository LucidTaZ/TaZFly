using UnityEngine;

public class AudioPitchBySpeed : MonoBehaviour {

	public float ScaleFactor = 0.03f; // Factor to convert speed into pitch increment
	public float NeutralSpeed = 5f;
	float startPitch;

	void Start () {
		startPitch = GetComponent<AudioSource>().pitch;
	}
	
	void Update () {
		//float speed = GetComponent<Rigidbody>().velocity.magnitude; // Could use this but I think it would be higher during steering, which would feel unnatural
		float speed = GetComponent<Rigidbody>().velocity.z;
		GetComponent<AudioSource>().pitch = startPitch + ((speed - NeutralSpeed) * ScaleFactor);
	}
}
