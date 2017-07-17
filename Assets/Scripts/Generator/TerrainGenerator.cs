using UnityEngine;

[RequireComponent(typeof(IBiome))]
abstract public class TerrainGenerator : MonoBehaviour
{
	public GameObject HeightNoise;
	INoise2D heightNoise;

	protected IBiome biomeGenerator;

	public void Awake () {
		heightNoise = HeightNoise.GetComponent<INoise2D>();
		if (heightNoise == null) {
			Debug.LogError("Referenced HeightNoise gameobject has no INoise2D component.");
		}

		biomeGenerator = GetComponent<IBiome>();
		biomeGenerator.Initialize();

		GameObject terrain = Generate();
		terrain.transform.parent = gameObject.transform;
	}

	protected abstract GameObject Generate();

	/**
	 * Generate the raw heightmap data
	 *
	 * Each point must lie between 0 and 1.
	 */
	protected float[,] GenerateHeightmap (int ResolutionX, int ResolutionZ, float Width, float Length) {
		float[,] heightmap = new float[ResolutionZ, ResolutionX];

		for (int z = 0; z < ResolutionZ; z++) {
			float zCoordinate = z * Length / ResolutionZ;
			for (int x = 0; x < ResolutionX; x++) {
				float xCoordinate = x * Width / ResolutionX;
				Vector2 groundCoordinates = new Vector2(xCoordinate, zCoordinate);
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
