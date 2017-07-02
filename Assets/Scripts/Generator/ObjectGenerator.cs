using UnityEngine;
using System.Collections.Generic;

public class ObjectGenerator : MonoBehaviour {

	public float LevelWidth = 50f;
	public float LevelLength = 150f;

	public List<GameObject> StructuralObjects;
	public List<GameObject> DynamicObjects;

	public float StructureDensityInv = 750f; // Reciprocal of structures per square meter
	public float DynamicDensityInv = 750f; // Reciprocal of dynamics per square meter

	public Rect ProtectedZone; // No-spawn zone

	public void Awake () {
		GameTerrain terrain = findTerrain();
		Rect boundary = new Rect(new Vector2(-LevelWidth / 2f, 0f), new Vector2(LevelWidth, LevelLength));

		List<GameObject> objects = Generate(terrain, boundary);
		foreach (GameObject obj in objects) {
			obj.transform.parent = gameObject.transform;
		}
	}

	/**
	 * Generate the objects that are present (cannons, barrels etc)
	 */
	public List<GameObject> Generate (GameTerrain terrain, Rect boundary) {
		float totalSurfaceArea = boundary.width * boundary.height;
		float protectedSurfaceArea = ProtectedZone.width * ProtectedZone.height;
		float usableSurfaceArea = totalSurfaceArea - protectedSurfaceArea;
		if (usableSurfaceArea < 0f) {
			throw new UnityException("Negative surface area, check settings.");
		}
		int nStructures = (int)Mathf.Round(usableSurfaceArea / StructureDensityInv);
		int nDynamics = (int)Mathf.Round(usableSurfaceArea / DynamicDensityInv);
		List<GameObject> objects = generateStructuralObjects(terrain, boundary, nStructures);
		objects.AddRange(generateDynamicObjects(terrain, boundary, nDynamics));
		return objects;
	}
	
	GameObject generateObject (IList<GameObject> pool, GameTerrain terrain, Rect boundary) {
		GameObject result = Instantiate(pool[Random.Range(0, pool.Count)]);
		Vector2 sampled;
		do {
			sampled = new Vector2(
				Random.Range(boundary.xMin, boundary.xMax),
				Random.Range(boundary.yMin, boundary.yMax)
			);
		} while (isInsideProtectedZone(sampled));
		result.transform.position = terrain.RaycastDownto(sampled);
		return result;
	}
	
	/**
	 * Generate the static (immovable) objects that are present
	 * 
	 * They may be embedded into the ground a bit.
	 */
	List<GameObject> generateStructuralObjects (GameTerrain terrain, Rect boundary, int amount) {
		List<GameObject> result = new List<GameObject>();
		for (int i = 0; i < amount; i++) {
			GameObject current = generateObject(StructuralObjects, terrain, boundary);
			current.transform.position += new Vector3(0f, -0.1f, 0f);
			result.Add(current);
		}
		return result;
	}
	
	/**
	 * Generate the dynamic (moving) objects that are present
	 * 
	 * They will not be empedded into the ground, but may spawn unnaturally above it.
	 */
	List<GameObject> generateDynamicObjects (GameTerrain terrain, Rect boundary, int amount) {
		List<GameObject> result = new List<GameObject>();
		for (int i = 0; i < amount; i++) {
			GameObject current = generateObject(DynamicObjects, terrain, boundary);
			current.transform.position += new Vector3(0f, 1f, 0f); // Avoid immediate terrain collision (Placing the object using a sweep does not always work: MeshColliders are not supported.
			result.Add(current);
		}
		return result;
	}

	bool isInsideProtectedZone (Vector2 coordinate) {
		return ProtectedZone.Contains(coordinate);
	}

	GameTerrain findTerrain () {
		GameTerrain terrain;
		foreach (GameObject obj in FindObjectsOfType<GameObject>()) {
			if ((terrain = obj.GetComponent<GameTerrain>()) != null) {
				return terrain;
			}
		}
		throw new MissingComponentException("GameTerrain not found, either have a GameTerrain component somewhere, or generate one using the TerrainGenerator, which has to execute before the ObjectGenerator!");
	}
}
