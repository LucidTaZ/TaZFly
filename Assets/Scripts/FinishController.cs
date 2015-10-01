using UnityEngine;
using System.Linq; // For Contains() method

public class FinishController : MonoBehaviour {

	protected Collider[] PlayerColliders;
	private bool finished;

	public void Load (GameObject playerShip) {
		PlayerColliders = playerShip.GetComponents<Collider>();
		finished = false;
	}

	void OnTriggerEnter (Collider other) {
		if (!finished && PlayerColliders.Contains<Collider>(other)) {
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayerFinished();
			finished = true;
		}
	}
	
}
