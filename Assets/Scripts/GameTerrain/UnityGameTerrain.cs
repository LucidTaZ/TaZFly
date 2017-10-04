using UnityEngine;

/**
 * Uses the Unity Terrain component
 */
[RequireComponent(typeof(Terrain))]
public class UnityGameTerrain : MonoBehaviour, GameTerrain {
	Terrain terrain;

	void Awake () {
		terrain = GetComponent<Terrain>();
	}

	public Vector3 RaycastDownto (Vector2 groundCoordinates, out bool hit) {
		Vector3 coordinates = new Vector3(groundCoordinates.x, 0.0f, groundCoordinates.y);
		float y = terrain.SampleHeight(coordinates);
		coordinates.y = y;
		// TODO: How can hit become false..?

		hit = true;
		return coordinates;
	}
}
