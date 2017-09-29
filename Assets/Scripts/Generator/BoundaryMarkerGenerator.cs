using System.Collections.Generic;
using UnityEngine;

public class BoundaryMarkerGenerator : MonoBehaviour, IChunkCreationModule {
	public float LevelWidth = 50f;
	public float LevelLength = 150f;

	public GameObject MarkerPrefab;

	public float SpaceBetween = 10.0f;

	ChunkRegistry chunkRegistry;

	Rect levelBoundary;

	void Awake () {
		levelBoundary = new Rect(new Vector2(-LevelWidth / 2f, 0f), new Vector2(LevelWidth, LevelLength));
	}

	public List<GameObject> Generate (Rect chunkBoundary) {
		List<GameObject> result = new List<GameObject>();
		for (float z = levelBoundary.yMin; z < levelBoundary.yMax; z += SpaceBetween) {
			Vector2 leftMarkerGroundPosition = new Vector2(levelBoundary.xMin, z);
			if (chunkBoundary.Contains(leftMarkerGroundPosition)) {
				GameObject left = Instantiate(MarkerPrefab);
				left.transform.localPosition += chunkRegistry.RaycastDownto(leftMarkerGroundPosition);
				result.Add(left);
			}

			Vector2 rightMarkerGroundPosition = new Vector2(levelBoundary.xMax, z);
			if (chunkBoundary.Contains(rightMarkerGroundPosition)) {
				GameObject right = Instantiate(MarkerPrefab);
				right.transform.localPosition += chunkRegistry.RaycastDownto(rightMarkerGroundPosition);
				result.Add(right);
			}
		}
		return result;
	}

	public void AddChunkContents (GameObject chunk, ChunkCreationContext context)
	{
		GameObject collection = new GameObject("Boundary markers");
		foreach (GameObject marker in Generate(context.GroundBoundary)) {
			marker.transform.parent = collection.transform;
		}
		collection.transform.parent = chunk.transform;
	}

	public void SetChunkRegistry (ChunkRegistry registry) {
		chunkRegistry = registry;
	}
}
