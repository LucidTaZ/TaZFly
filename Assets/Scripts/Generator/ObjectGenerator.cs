using UnityEngine;
using System.Collections.Generic;

public class ObjectGenerator : MonoBehaviour, IChunkCreationModule {
	public List<GameObject> StructuralObjects;
	public List<GameObject> DynamicObjects;

	public float StructureDensityInv = 750f; // Reciprocal of structures per square meter
	public float DynamicDensityInv = 750f; // Reciprocal of dynamics per square meter

	public Rect ProtectedZone; // No-spawn zone

	public void AddChunkContents (GameObject chunk, ChunkCreationContext context)
	{
		GameObject collection = new GameObject("Generated objects");
		foreach (GameObject marker in Generate(context.GroundBoundary)) {
			marker.transform.parent = collection.transform;
		}
		collection.transform.parent = chunk.transform;
	}

	/**
	 * Generate the objects that are present (cannons, barrels etc)
	 */
	public List<GameObject> Generate (Rect chunkBboundary) {
		float totalSurfaceArea = chunkBboundary.width * chunkBboundary.height;
		float protectedSurfaceArea = ProtectedZone.width * ProtectedZone.height;
		float usableSurfaceArea = totalSurfaceArea - protectedSurfaceArea;
		if (usableSurfaceArea < 0f) {
			throw new UnityException("Negative surface area, check settings.");
		}
		int nStructures = (int)Mathf.Round(usableSurfaceArea / StructureDensityInv);
		int nDynamics = (int)Mathf.Round(usableSurfaceArea / DynamicDensityInv);
		List<GameObject> objects = generateStructuralObjects(chunkBboundary, nStructures);
		objects.AddRange(generateDynamicObjects(chunkBboundary, nDynamics));
		return objects;
	}
	
	GameObject generateObject (IList<GameObject> pool, Rect chunkBboundary) {
		GameObject result = Instantiate(pool[Random.Range(0, pool.Count)]);
		Vector2 sampled;
		do {
			sampled = new Vector2(
				Random.Range(chunkBboundary.xMin, chunkBboundary.xMax),
				Random.Range(chunkBboundary.yMin, chunkBboundary.yMax)
			);
		} while (isInsideProtectedZone(sampled));
		result.transform.position = TerrainRegistry.RaycastDownto(sampled);
		return result;
	}
	
	/**
	 * Generate the static (immovable) objects that are present
	 * 
	 * They may be embedded into the ground a bit.
	 */
	List<GameObject> generateStructuralObjects (Rect chunkBboundary, int amount) {
		List<GameObject> result = new List<GameObject>();
		for (int i = 0; i < amount; i++) {
			GameObject current = generateObject(StructuralObjects, chunkBboundary);
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
	List<GameObject> generateDynamicObjects (Rect chunkBboundary, int amount) {
		List<GameObject> result = new List<GameObject>();
		for (int i = 0; i < amount; i++) {
			GameObject current = generateObject(DynamicObjects, chunkBboundary);
			current.transform.position += new Vector3(0f, 1f, 0f); // Avoid immediate terrain collision (Placing the object using a sweep does not always work: MeshColliders are not supported.
			result.Add(current);
		}
		return result;
	}

	bool isInsideProtectedZone (Vector2 coordinate) {
		return ProtectedZone.Contains(coordinate);
	}
}
