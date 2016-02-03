using UnityEngine;

public class CollisionExplosion : MonoBehaviour {
	// Prefab from the "Detonator" framework (asset store)
	// Make sure the prefab does not have "Explode on start" enabled. We tweak it a bit before initiating the explosion
	public GameObject Detonator;

	bool actionPerformed;

	void Start () {
		actionPerformed = false;
	}

	void OnCollisionEnter (Collision collision) {
		if (!actionPerformed) {
			actionPerformed = true;

			CameraShake shake;
			if ((shake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>()) != null) {
				shake.ShakeAtLocation(transform.position);
			}

			gameObject.GetComponent<MeshRenderer>().enabled = false;
			Collider[] colliders = gameObject.GetComponents<Collider>();
			foreach (Collider thisCollider in colliders) {
				thisCollider.enabled = false;
			}

			// Explode the Detonator framework explosion
			if (Detonator) {
				GameObject detonatorObject = GameObject.Instantiate(Detonator);
				detonatorObject.transform.position = transform.position;
				//Destroy(detonatorObject.GetComponent<DetonatorSound>());
				//detonatorObject.GetComponent<DetonatorSound>().enabled = false;
				detonatorObject.GetComponent<Detonator>().detail = determineDetail();
				detonatorObject.GetComponent<Detonator>().Explode();
				DestroyObject(gameObject, detonatorObject.GetComponent<Detonator>().destroyTime); // Destroy gameobject when effect is approximately over
			} else {
				DestroyObject(gameObject, 10); // Destroy gameobject when effect is approximately over
			}
		}
	}

	float determineDetail () {
		// Return a value from 0 (no sound at all) to 1 (make sound). The Audio component of the Detonator is calibrated to react on this.

		float cutoffDistance = 100f;

		Transform camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		float distanceSq = (camera.position - transform.position).sqrMagnitude;
		if (distanceSq > cutoffDistance * cutoffDistance) {
			return 0;
		} else {
			return 1;
		}
	}
}
