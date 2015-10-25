using UnityEngine;
using System.Collections.Generic;

public class ObjectGenerator : MonoBehaviour {

	public List<GameObject> StructuralObjects;
	public List<GameObject> DynamicObjects;

	public int NStructures = 10;
	public int NDynamics = 10;

	/**
	 * Generate the objects that are present (cannons, barrels etc)
	 */
	public List<GameObject> Generate (Terrain terrain, Rect boundary) {
		List<GameObject> objects = generateStructuralObjects(terrain, boundary);
		objects.AddRange(generateDynamicObjects(terrain, boundary));
		return objects;
	}
	
	GameObject generateObject (IList<GameObject> pool, Terrain terrain, Rect boundary) {
		GameObject result = Instantiate(pool[Random.Range(0, pool.Count)]);
		float x = Random.Range(boundary.xMin, boundary.xMax);
		float z = Random.Range(boundary.yMin, boundary.yMax);
		float y = terrain.SampleHeight(new Vector3(x, 0f, z));
		result.transform.position = new Vector3(x, y, z);
		return result;
	}
	
	/**
	 * Generate the static (immovable) objects that are present
	 * 
	 * They may be embedded into the ground a bit.
	 */
	List<GameObject> generateStructuralObjects (Terrain terrain, Rect boundary) {
		List<GameObject> result = new List<GameObject>();
		for (int i = 0; i < NStructures; i++) {
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
	List<GameObject> generateDynamicObjects (Terrain terrain, Rect boundary) {
		List<GameObject> result = new List<GameObject>();
		for (int i = 0; i < NDynamics; i++) {
			GameObject current = generateObject(DynamicObjects, terrain, boundary);
			current.transform.position += new Vector3(0f, 1f, 0f); // Avoid immediate terrain collision (Placing the object using a sweep does not always work: MeshColliders are not supported.
			result.Add(current);
		}
		return result;
	}
}
