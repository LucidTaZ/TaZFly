using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalKeyHandler : MonoBehaviour {
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			Debug.Log("Player quit via keyboard.");
			SceneManager.LoadScene("MainMenu");
		}
	}
}
