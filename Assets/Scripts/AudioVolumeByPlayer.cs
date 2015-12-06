using UnityEngine;

/**
 * Change audio volume based on whether the source is the player
 *
 * We simply look at whether we have a PlayerController. That might need to be changed at some point, but for now it
 * works.
 */
public class AudioVolumeByPlayer : MonoBehaviour {

	public float PlayerVolume = 1.0f;
	public float NonPlayerVolume = 1.0f;

	// Use this for initialization
	void Start () {
		if (GetComponent<PlayerController>()) {
			GetComponent<AudioSource>().volume = PlayerVolume;
		} else {
			GetComponent<AudioSource>().volume = NonPlayerVolume;
		}
	}
}
