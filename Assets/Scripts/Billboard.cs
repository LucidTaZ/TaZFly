using UnityEngine;

public class Billboard : MonoBehaviour {

	void Update () {
		transform.LookAt(2*transform.position - Camera.main.transform.position); // transform.LookAt(Camera.main.transform.position) looks exactly in the opposite direction!
	}

}
