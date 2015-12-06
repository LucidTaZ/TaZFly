using UnityEngine;

/**
 * Single interface for the game to provide data to the various GUI components
 */
public class GUIController : MonoBehaviour {
	public GUIText HitpointsText;
	public GUIText SpeedText;
	public GUIText EnemiesText;

	Hitpoints hitpoints;
	Rigidbody subject;

	public void SetPlayer(GameObject player) {
		hitpoints = player.GetComponent<Hitpoints>();
		subject = player.GetComponent<Rigidbody>();
	}

	void Update () {
		if (canUpdate()) {
			HitpointsText.text = System.String.Format("Hitpoints: {0:P0}", Mathf.Max(0, hitpoints.GetRelativeHitpoints()));
			SpeedText.text = System.String.Format("Speed: {0:N2}", subject.velocity.z);
			EnemiesText.text = System.String.Format("Enemies: {0:N0}", GameObject.FindGameObjectsWithTag("Ship").Length - 1); // This can probably be more efficient. Maybe via a listener approach?
		}
	}

	bool canUpdate() {
		return subject != null; // Happens during level reload
	}
}
