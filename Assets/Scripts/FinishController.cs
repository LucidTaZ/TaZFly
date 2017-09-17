using UnityEngine;
using System.Linq; // For Contains() method

public class FinishController : MonoBehaviour {
	protected Collider[] PlayerColliders;
	bool finished;

	GameController gameController;

	void Awake () {
		gameController = GameController.InstanceIfExists();
	}

	public void Load (GameObject playerShip) {
		PlayerColliders = playerShip.GetComponents<Collider>();
		finished = false;
	}

	void OnTriggerEnter (Collider other) {
		if (!finished && PlayerColliders.Contains<Collider>(other)) {
			if (gameController != null) {
				gameController.PlayerFinished();
			}
			finished = true;
		}
	}
}
