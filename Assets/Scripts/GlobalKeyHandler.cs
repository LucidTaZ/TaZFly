using UnityEngine;

public class GlobalKeyHandler : MonoBehaviour {
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			Debug.Log("Player quit via keyboard.");
			Application.Quit();
		}
	}
}
