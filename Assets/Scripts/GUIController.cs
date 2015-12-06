using UnityEngine;

/**
 * Single interface for the game to provide data to the various GUI components
 */
public class GUIController : MonoBehaviour {
	public GUIText HitpointsText;
	public GUIText SpeedText;

	Hitpoints hitpoints;
	Rigidbody subject;

	public void SetPlayer(GameObject player) {
		hitpoints = player.GetComponent<Hitpoints>();
		subject = player.GetComponent<Rigidbody>();
	}

	void Update () {
		if (subject != null) { // Happens during level reload
			HitpointsText.text = System.String.Format("Hitpoints: {0:P0}", Mathf.Max(0, hitpoints.GetRelativeHitpoints()));
			SpeedText.text = System.String.Format("Speed: {0:N2}", subject.velocity.z);
		}
	}
}
