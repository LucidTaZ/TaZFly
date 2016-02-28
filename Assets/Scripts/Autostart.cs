using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Automatically proceed to the next scene, while constructing the game framework on the fly
 */
public class Autostart : MonoBehaviour {

	public GameObject gameFramework;

	void Start () {
		Debug.Log("Autostarting...");
		GameObject instance = Instantiate(gameFramework);
		DontDestroyOnLoad(instance);
		instance.SetActive(false);
		Debug.Log("Instantiated framework.");
		if (SceneManager.sceneCountInBuildSettings < SceneManager.GetActiveScene().buildIndex + 1) {
			Debug.LogError("Not enough scenes found.");
		} else {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			Camera.main.gameObject.SetActive(false); // Prevent double audio listeners etc.
			//instance.SetActive(true);
		}
	}
}
