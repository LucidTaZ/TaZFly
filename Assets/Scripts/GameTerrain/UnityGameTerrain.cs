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

	public Vector3 RaycastDownto (Vector2 coordinates) {
		float y = terrain.SampleHeight(coordinates);
		return new Vector3(coordinates.x, y, coordinates.y);
	}
}
