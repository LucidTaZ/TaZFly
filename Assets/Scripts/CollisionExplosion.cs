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

			ShakePositional.ShakeAtLocation(transform.position, 10, 1, 0.1f, 0.5f);

			gameObject.GetComponent<MeshRenderer>().enabled = false;
			Collider[] colliders = gameObject.GetComponents<Collider>();
			foreach (Collider thisCollider in colliders) {
				thisCollider.enabled = false;
			}

			// Explode the Detonator framework explosion
			if (Detonator) {
				GameObject detonatorObject = GameObject.Instantiate(Detonator);
				detonatorObject.transform.position = transform.position;
				detonatorObject.GetComponent<Detonator>().Explode();
				DestroyObject(gameObject, detonatorObject.GetComponent<Detonator>().destroyTime); // Destroy gameobject when effect is approximately over
			} else {
				DestroyObject(gameObject, 10); // Destroy gameobject when effect is approximately over
			}
		}
	}

}
