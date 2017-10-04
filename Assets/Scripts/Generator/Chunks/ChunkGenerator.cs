using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ChunkRegistry))]
public class ChunkGenerator : MonoBehaviour {
	public GameObject[] ChunkCreationModules;
	List<IChunkCreationModule> chunkCreationModules = new List<IChunkCreationModule>();

	public float GenerationUpdateInterval = 1.0f;
	float lastGenerationUpdateCheck = -99f;
	GridMask chunkSpawnMask; // Tells where we want to have chunks, relative to current coordinates
	GridMask chunkDespawnMask; // Any chunks outside this area are up for a despawn

	[Tooltip("Optional player ship, will be taken from GameController if it is not set.")]
	public GameObject ShipToFollow;

	ChunkRegistry chunkRegistry;

	virtual protected void Awake () {
		chunkRegistry = GetComponent<ChunkRegistry>();

		foreach (GameObject chunkCreationModuleHolder in ChunkCreationModules) {
			IChunkCreationModule module = chunkCreationModuleHolder.GetComponent<IChunkCreationModule>();
			Debug.Assert(module != null);
			chunkCreationModules.Add(module);
		}

		assignChunkRegistryToCreationModules();

		chunkSpawnMask = GridMask.CreateSquare(1).Translate(new GridCoordinates(0, 1));
		chunkDespawnMask = chunkSpawnMask.Dilate(1);
	}


	void Start () {
		GameController gameController = GameController.InstanceIfExists();
		if (gameController != null) {
			// ShipToFollow can be set via the editor (is the case for MainMenu) or automatically (is the case for
			// spawned player ships in regular levels)
			ShipToFollow = gameController.PlayerShip;
		}

		// Generate initial chunks
		generate(new GridCoordinates(-1, 0));
		generate(new GridCoordinates(0, 0));
	}

	void Update () {
		if (Time.time > lastGenerationUpdateCheck + GenerationUpdateInterval) {
			updateGeneration();
			lastGenerationUpdateCheck = Time.time;
		}
	}

	/**
	 * Spawn new chunks, if needed
	 * Despawn old chunks, if needed
	 */
	void updateGeneration () {
		Vector3 playerPosition = ShipToFollow.transform.position;
		Vector2 playerGroundPosition = new Vector2(playerPosition.x, playerPosition.z);
		GridCoordinates playerGridPosition = Chunk.groundPositionToGridCoordinates(playerGroundPosition);

		foreach (GridCoordinates spawnOffset in chunkSpawnMask) {
			GridCoordinates spawnCoordinates = playerGridPosition + spawnOffset;
			if (!chunkRegistry.HasAt(spawnCoordinates)) {
				generate(spawnCoordinates);
			}
		}

		GridMask chunkDespawnArea = chunkDespawnMask.Translate(playerGridPosition);
		foreach (GridCoordinates coords in chunkRegistry.GetAllOutside(chunkDespawnArea)) {
			despawn(coords);
		}
	}

	GameObject generate (GridCoordinates coords) {
		Vector2 originGroundPosition = Chunk.gridCoordinatesToGroundPosition(coords);
		Rect groundBoundary = new Rect(originGroundPosition, new Vector2(Chunk.WIDTH, Chunk.LENGTH));
		ChunkCreationContext context = new ChunkCreationContext(coords, groundBoundary);

		GameObject result = new GameObject("Generated Chunk");
		result.transform.position = Chunk.gridCoordinatesToWorldPosition(coords);
		result.transform.parent = transform;
		chunkRegistry.Register(result, coords);
		applyCreationModules(result, context);
		return result;
	}

	void despawn (GridCoordinates coords) {
		GameObject chunk = chunkRegistry.GetChunk(coords);
		Destroy(chunk);
		chunkRegistry.ForgetAt(coords);
	}

	void assignChunkRegistryToCreationModules () {
		foreach (IChunkCreationModule module in chunkCreationModules) {
			module.SetChunkRegistry(chunkRegistry);
		}
	}

	void applyCreationModules (GameObject chunk, ChunkCreationContext context) {
		foreach (IChunkCreationModule module in chunkCreationModules) {
			module.AddChunkContents(chunk, context);
		}
	}
}
