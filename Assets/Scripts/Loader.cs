using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject template;

	void Awake () {
		if (!GameObject.FindGameObjectWithTag("GameFramework")) {
			GameObject instance = Instantiate(template);
			DontDestroyOnLoad(instance);
			Debug.Log("Instantiated the game framework.");
		} else {
			Debug.Log("Skipping instantiation of the game framework because it is already there.");
		}
	}
}
