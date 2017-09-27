using UnityEngine;

[RequireComponent(typeof(IBiome))]
abstract public class TerrainGenerator : MonoBehaviour
{
	// Quads per chunk
	public int ResolutionX = 8;
	public int ResolutionZ = 8;

	public GameObject HeightNoise;
	INoise2D heightNoise;

	public float GenerationUpdateInterval = 1.0f;
	float lastGenerationUpdateCheck = -99f;
	Vector2[] generationUpdateFeelers;

	protected IBiome biomeGenerator;

	GameController gameController;

	virtual protected void Awake () {
		heightNoise = HeightNoise.GetComponent<INoise2D>();
		if (heightNoise == null) {
			Debug.LogError("Referenced HeightNoise gameobject has no INoise2D component.");
		}

		biomeGenerator = GetComponent<IBiome>();
		biomeGenerator.Initialize();

		TerrainRegistry.Clear();

		GameObject initialTerrainLeft = Generate(new Vector3(-Chunk.WIDTH, 0, 0));
		initialTerrainLeft.transform.parent = gameObject.transform;
		TerrainRegistry.Register(initialTerrainLeft, new GridCoordinates(-1, 0));

		GameObject initialTerrainRight = Generate(new Vector3(0, 0, 0));
		initialTerrainRight.transform.parent = gameObject.transform;
		TerrainRegistry.Register(initialTerrainRight, new GridCoordinates(0, 0));

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
	}

	protected abstract GameObject Generate(Vector3 offset);

	void Update () {
		if (gameController != null && Time.time > lastGenerationUpdateCheck + GenerationUpdateInterval) {
			updateGeneration();
			lastGenerationUpdateCheck = Time.time;
		}
	}

	/**
	 * Spawn new terrain tiles, if needed
	 */
	void updateGeneration () {
		Vector3 playerPosition = gameController.PlayerPosition;
		Vector2 playerGroundPosition = new Vector2(playerPosition.x, playerPosition.z);
		foreach (Vector2 feeler in generationUpdateFeelers) {
			Vector2 potentialSpawnPosition = playerGroundPosition + feeler * 1.001f; // Adjust a bit to avoid probing just along a seam
			GridCoordinates potentialSpawnCoords = Chunk.groundPositionToGridCoordinates(potentialSpawnPosition);
			if (!TerrainRegistry.HasAt(potentialSpawnCoords)) {
				Vector3 offset = Chunk.gridCoordinatesToWorldPosition(
					potentialSpawnCoords
				);
				GameObject generated = Generate(offset);
				generated.transform.parent = transform;
				TerrainRegistry.Register(generated, potentialSpawnCoords);
			}
		}
	}

	/**
	 * Generate the raw heightmap data
	 *
	 * Each point must lie between 0 and 1.
	 */
	protected float[,] GenerateHeightmap (Vector2 groundOffset) {
		float[,] heightmap = new float[ResolutionZ, ResolutionX];

		for (int z = 0; z < ResolutionZ; z++) {
			float zCoordinate = z * Chunk.LENGTH / (ResolutionZ - 1);
			for (int x = 0; x < ResolutionX; x++) {
				float xCoordinate = x * Chunk.WIDTH / (ResolutionX - 1);
				Vector2 groundCoordinates = new Vector2(xCoordinate, zCoordinate) + groundOffset;
				float hillyness = biomeGenerator.GetHillyness(groundCoordinates);
				float detailHeight = heightNoise.Sample(groundCoordinates);

				// Hillyness makes the terrain higher in general and also more varying in terms of smaller hills
				// In what strength the hillyness has a flat influence to the general height, is the meaning of the blend factor
				heightmap[z, x] = Mathf.Lerp(detailHeight * hillyness, hillyness, 0.2f);
			}
		}

		return heightmap;
	}
}
