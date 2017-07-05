using UnityEngine;

/**
 * Uses a Mesh component
 */
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Collider))]
public class MeshGameTerrain : MonoBehaviour, GameTerrain {
	Collider terrainCollider;

	void Awake () {
		terrainCollider = GetComponent<Collider>();
	}

	public Vector3 RaycastDownto (Vector2 coordinates) {
		Vector3 rayOrigin = new Vector3(
			coordinates.x,
			999.0f,
			coordinates.y
		);

		RaycastHit hitInfo;
		if (!terrainCollider.Raycast(new Ray(rayOrigin, Vector3.down), out hitInfo, 9999.0f)) {
			Debug.LogError("Could not find terrain at coordinates: " + coordinates);
		}
		return hitInfo.point;
	}
}
