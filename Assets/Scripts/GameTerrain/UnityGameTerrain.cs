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

	public Vector3 RaycastDownto (Vector2 coordinates, out bool hit) {
		float y = terrain.SampleHeight(coordinates);
		// TODO: How can hit become false..?

		hit = true;
		return new Vector3(coordinates.x, y, coordinates.y);
	}
}
