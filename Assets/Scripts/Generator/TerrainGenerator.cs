using UnityEngine;

abstract public class TerrainGenerator : MonoBehaviour, IChunkCreationModule
{
	// Quads per chunk
	public int ResolutionX = 8;
	public int ResolutionZ = 8;

	public GameObject HeightNoise;
	INoise2D heightNoise;

	[Tooltip("0 = only HeightNoise, 1 = only Biome Elevation")]
	[Range(0.0f, 1.0f)]
	public float HeightElevationBlendFactor = 0.3f;

	public GameObject BiomeGenerator;
	protected IBiome biomeGenerator;

	protected ChunkRegistry chunkRegistry;

	virtual protected void Awake () {
		heightNoise = HeightNoise.GetComponent<INoise2D>();
		if (heightNoise == null) {
			Debug.LogError("Referenced HeightNoise gameobject has no INoise2D component.");
		}

		biomeGenerator = BiomeGenerator.GetComponent<IBiome>();
		Debug.Assert(biomeGenerator != null);
		biomeGenerator.Initialize();
	}

	public void AddChunkContents (GameObject chunk, ChunkCreationContext context)
	{
		GameObject terrain = Generate(chunk.transform.position);
		terrain.transform.parent = chunk.transform;
	}

	protected abstract GameObject Generate(Vector3 offset);

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
				float elevation = biomeGenerator.GetElevation(groundCoordinates);
				float detailHeight = heightNoise.Sample(groundCoordinates);

				// Hillyness makes the terrain higher in general and also more varying in terms of smaller hills
				// In what strength the hillyness has a flat influence to the general height, is the meaning of the blend factor
				heightmap[z, x] = Mathf.Lerp(detailHeight, elevation, HeightElevationBlendFactor);
			}
		}

		return heightmap;
	}

	public void SetChunkRegistry (ChunkRegistry registry) {
		chunkRegistry = registry;
	}
}
