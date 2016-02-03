using UnityEngine;

public class ExplosionDistanceCutoff : DetonatorComponent {
	public float CutoffDistance = 100f;

	override public void Init () {

	}

	override public void Explode () {
		GetComponent<Detonator>().detail = determineDetail();
	}

	float determineDetail () {
		// Return a value from 0 (no sound at all) to 1 (make sound). The Audio component of the Detonator is calibrated to react on this.

		Transform camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		float distanceSq = (camera.position - transform.position).sqrMagnitude;
		if (distanceSq > CutoffDistance * CutoffDistance) {
			return 0;
		} else {
			return 1;
		}
	}
}
