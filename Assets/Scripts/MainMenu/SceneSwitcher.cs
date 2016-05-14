using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneSwitcher : MonoBehaviour {

	public String SceneToLoad;

	public void SwitchToScene () {
		SceneManager.LoadScene(SceneToLoad);
	}
}
