using UnityEngine;
using System.Collections.Generic;

public class ChunkGenerator : MonoBehaviour {
	public GameObject[] ChunkCreationModules;
	List<IChunkCreationModule> chunkCreationModules = new List<IChunkCreationModule>();

	public float GenerationUpdateInterval = 1.0f;
	float lastGenerationUpdateCheck = -99f;
	Vector2[] generationUpdateFeelers;

	GameController gameController;

	virtual protected void Awake () {
		foreach (GameObject chunkCreationModuleHolder in ChunkCreationModules) {
			IChunkCreationModule module = chunkCreationModuleHolder.GetComponent<IChunkCreationModule>();
			Debug.Assert(module != null);
			chunkCreationModules.Add(module);
		}

		generationUpdateFeelers = new []{
			new Vector2(-Chunk.WIDTH / 2, Chunk.LENGTH    ), new Vector2(Chunk.WIDTH / 2, Chunk.LENGTH),
			new Vector2(-Chunk.WIDTH / 2, Chunk.LENGTH / 2), new Vector2(Chunk.WIDTH / 2, Chunk.LENGTH / 2),
			new Vector2(-Chunk.WIDTH,     Chunk.LENGTH    ), new Vector2(Chunk.WIDTH,     Chunk.LENGTH),
			new Vector2(-Chunk.WIDTH,     Chunk.LENGTH / 2), new Vector2(Chunk.WIDTH,     Chunk.LENGTH / 2),
			new Vector2(-2 * Chunk.WIDTH, Chunk.LENGTH    ), new Vector2(2 * Chunk.WIDTH, Chunk.LENGTH),
			new Vector2(-2 * Chunk.WIDTH, Chunk.LENGTH / 2), new Vector2(2 * Chunk.WIDTH, Chunk.LENGTH / 2),
		};
	}

	void Start () {
		gameController = GameController.InstanceIfExists();

		TerrainRegistry.Clear();

		// Generate initial chunks
		generate(new GridCoordinates(-1, 0));
		generate(new GridCoordinates(0, 0));
	}

	void Update () {
		if (gameController != null && Time.time > lastGenerationUpdateCheck + GenerationUpdateInterval) {
			updateGeneration();
			lastGenerationUpdateCheck = Time.time;
		}
	}

	/**
	 * Spawn new chunks, if needed
	 */
	void updateGeneration () {
		// TODO: Find a way to make this work in the main menu, where we have no GameController
		Vector3 playerPosition = gameController.PlayerPosition;
		Vector2 playerGroundPosition = new Vector2(playerPosition.x, playerPosition.z);
		foreach (Vector2 feeler in generationUpdateFeelers) {
			Vector2 potentialSpawnPosition = playerGroundPosition + feeler * 1.001f; // Adjust a bit to avoid probing just along a seam
			GridCoordinates potentialSpawnCoords = Chunk.groundPositionToGridCoordinates(potentialSpawnPosition);
			if (!TerrainRegistry.HasAt(potentialSpawnCoords)) {
				generate(potentialSpawnCoords);
			}
		}
	}

	GameObject generate (GridCoordinates coords) {
		Vector2 originGroundPosition = Chunk.gridCoordinatesToGroundPosition(coords);
		Rect groundBoundary = new Rect(originGroundPosition, new Vector2(Chunk.WIDTH, Chunk.LENGTH));
		ChunkCreationContext context = new ChunkCreationContext(coords, groundBoundary);

		GameObject result = new GameObject("Generated Chunk");
		result.transform.position = Chunk.gridCoordinatesToWorldPosition(coords);
		result.transform.parent = transform;
		TerrainRegistry.Register(result, coords);
		applyCreationModules(result, context);
		return result;
	}

	void applyCreationModules (GameObject chunk, ChunkCreationContext context) {
		foreach (IChunkCreationModule module in chunkCreationModules) {
			module.AddChunkContents(chunk, context);
		}
	}
}
