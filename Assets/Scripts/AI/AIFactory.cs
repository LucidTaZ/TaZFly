using UnityEngine;
using System.Collections;

public class AIFactory {

	public void AttachAI (GameObject ship, float difficulty) {
		Debug.Log("Creating AI with difficulty: " + difficulty);

		AvoidingAI sc = ship.AddComponent<AvoidingAI>();

		sc.HorizontalMoveRate = 8;
		sc.VerticalMoveRate = 8;

		sc.LookAheadDistance = 40.0f * difficulty; // Difficult AI sees dangers earlier
		sc.LookDownAheadDistance = 5.0f / difficulty; // Difficult AI tries to get closer
	}

}
