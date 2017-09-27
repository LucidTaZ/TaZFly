using System.Collections.Generic;
using UnityEngine;

// TODO: Rename to ChunkRegistry and make non-static (make available via a GameObject)
public static class TerrainRegistry {
	static Dictionary<GridCoordinates, GameObject> terrainObjects = new Dictionary<GridCoordinates, GameObject>();

	public static IEnumerable<GameTerrain> FindAll () {
		foreach (GameObject obj in terrainObjects.Values) {
			yield return obj.GetComponentInChildren<GameTerrain>();
		}
	}

	public static GameTerrain FindAt (GridCoordinates coords) {
		if (!HasAt(coords)) {
			throw new UnityException("No terrain found at coordinates " + coords);
		}
		return findAtUnchecked(coords);
	}

	public static GameTerrain findAtUnchecked (GridCoordinates coords) {
		return terrainObjects[coords].GetComponentInChildren<GameTerrain>();
	}

	public static void Register (GameObject terrainObject, GridCoordinates coords) {
		terrainObjects[coords] = terrainObject;
	}

	// TODO: Remove once the class is non-static but managed via a GameObject instead
	public static void Clear () {
		terrainObjects.Clear();
	}

	public static Vector3 RaycastDownto (Vector2 groundPosition) {
		GridCoordinates coords = Chunk.groundPositionToGridCoordinates(groundPosition);
		if (!HasAt(coords)) {
			Debug.LogError("No terrain found at " + groundPosition + " (= grid " + coords + ")");
			return Vector3.zero;
		}
		GameTerrain terrain = findAtUnchecked(coords);
		return raycastDownto(terrain, groundPosition);
	}

	static Vector3 raycastDownto (GameTerrain terrain, Vector2 groundPosition) {
		Vector3 intersection;
		bool hit;
		intersection = terrain.RaycastDownto(groundPosition, out hit);
		if (!hit) {
			Debug.LogError("Terrain found but raycast did not intersect");
			return Vector3.zero;
		}
		return intersection;
	}

	public static bool HasAt (GridCoordinates coords) {
		return terrainObjects.ContainsKey(coords);
	}
}
