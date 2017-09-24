using System.Collections.Generic;
using UnityEngine;

public class BoundaryMarkerGenerator : MonoBehaviour {
	public float LevelWidth = 50f;
	public float LevelLength = 150f;

	public GameObject MarkerPrefab;

	public float SpaceBetween = 10.0f;

	void Awake () {
		Rect boundary = new Rect(new Vector2(-LevelWidth / 2f, 0f), new Vector2(LevelWidth, LevelLength));

		List<GameObject> objects = Generate(boundary);
		foreach (GameObject obj in objects) {
			obj.transform.SetParent(gameObject.transform, false);
		}
	}

	public List<GameObject> Generate (Rect boundary) {
		List<GameObject> result = new List<GameObject>();
		for (float z = boundary.yMin; z < boundary.yMax; z += SpaceBetween) {
			GameObject left = Instantiate(MarkerPrefab);
			GameObject right = Instantiate(MarkerPrefab);
			left.transform.localPosition += TerrainRegistry.RaycastDownto(new Vector2(boundary.xMin, z));
			right.transform.localPosition += TerrainRegistry.RaycastDownto(new Vector2(boundary.xMax, z));
			result.Add(left);
			result.Add(right);
		}
		return result;
	}
}
