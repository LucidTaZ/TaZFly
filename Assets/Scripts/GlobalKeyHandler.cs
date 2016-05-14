using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalKeyHandler : MonoBehaviour {
	void Update () {
		if (Input.GetAxis("Cancel") > .5f) {
			Debug.Log("Player quit via keyboard.");
			SceneManager.LoadScene("MainMenu");
		}
	}
}
