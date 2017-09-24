using System.Collections.Generic;
using UnityEngine;

public class TerrainRegistry {
	public static IEnumerable<GameTerrain> FindAll () {
		GameTerrain currentTerrain;
		foreach (GameObject obj in Object.FindObjectsOfType<GameObject>()) {
			if ((currentTerrain = obj.GetComponent<GameTerrain>()) != null) {
				yield return currentTerrain;
			}
		}
	}

	public static Vector3 RaycastDownto (Vector2 groundPosition) {
		bool wasHit;
		Vector3 result = raycastDownto(groundPosition, out wasHit);
		if (!wasHit) {
			Debug.LogError("No terrain found at " + groundPosition);
			return Vector3.zero;
		}
		return result;
	}

	static Vector3 raycastDownto (Vector2 groundPosition, out bool hit) {
		Vector3 intersection;
		foreach (GameTerrain terrain in FindAll()) {
			intersection = terrain.RaycastDownto(groundPosition, out hit);
			if (hit) {
				return intersection;
			}
		}
		hit = false;
		return Vector3.zero;
	}

	public static bool HasAt (Vector2 groundPosition) {
		bool wasHit;
		raycastDownto(groundPosition, out wasHit);
		return wasHit;
	}
}
