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
		Vector3 intersection;
		foreach (GameTerrain terrain in FindAll()) {
			intersection = terrain.RaycastDownto(groundPosition, out wasHit);
			if (wasHit) {
				return intersection;
			}
		}
		Debug.LogError("No terrain found at " + groundPosition);
		return Vector3.zero;
	}
}
