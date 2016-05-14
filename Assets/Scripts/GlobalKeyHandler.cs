using UnityEngine;

public class GlobalKeyHandler : MonoBehaviour {
	void Update () {
		if (Input.GetAxis("Cancel") > .5f) {
			// TODO: Return to main menu
			Debug.Log("Player quit via keyboard.");
			Application.Quit();
		}
	}
}
