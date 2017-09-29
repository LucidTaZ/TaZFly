using System.Collections.Generic;
using UnityEngine;

public class ChunkRegistry : MonoBehaviour {
	Dictionary<GridCoordinates, GameObject> terrainObjects = new Dictionary<GridCoordinates, GameObject>();

	public void Register (GameObject terrainObject, GridCoordinates coords) {
		terrainObjects[coords] = terrainObject;
	}

	public Vector3 RaycastDownto (Vector2 groundPosition) {
		GridCoordinates coords = Chunk.groundPositionToGridCoordinates(groundPosition);
		if (!HasAt(coords)) {
			Debug.LogError("No terrain found at " + groundPosition + " (= grid " + coords + ")");
			return Vector3.zero;
		}
		GameTerrain terrain = terrainObjects[coords].GetComponentInChildren<GameTerrain>();
		return raycastDownto(terrain, groundPosition);
	}

	Vector3 raycastDownto (GameTerrain terrain, Vector2 groundPosition) {
		Vector3 intersection;
		bool hit;
		intersection = terrain.RaycastDownto(groundPosition, out hit);
		if (!hit) {
			Debug.LogError("Terrain found but raycast did not intersect");
			return Vector3.zero;
		}
		return intersection;
	}

	public bool HasAt (GridCoordinates coords) {
		return terrainObjects.ContainsKey(coords);
	}
}
