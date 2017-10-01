using System.Collections.Generic;
using UnityEngine;

public class ChunkRegistry : MonoBehaviour {
	Dictionary<GridCoordinates, GameObject> terrainObjects = new Dictionary<GridCoordinates, GameObject>();

	public void Register (GameObject terrainObject, GridCoordinates coords) {
		terrainObjects[coords] = terrainObject;
	}

	public void ForgetAt (GridCoordinates coords) {
		terrainObjects.Remove(coords);
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

	public GameObject GetChunk (GridCoordinates coords) {
		return terrainObjects[coords];
	}

	public IEnumerable<GridCoordinates> GetAllOutside (ICollection<GridCoordinates> area) {
		// Copy list of present coordinates, because the caller will despawn chunks and therefore alter the data structure
		GridCoordinates[] presentCoordinates = new GridCoordinates[terrainObjects.Count];
		terrainObjects.Keys.CopyTo(presentCoordinates, 0);

		foreach (GridCoordinates coords in presentCoordinates) {
			if (!area.Contains(coords)) {
				yield return coords;
			}
		}
	}
}
