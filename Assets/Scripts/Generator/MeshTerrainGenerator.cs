using UnityEngine;

/**
 * Generates a Terrain using a mesh
 *
 * The result is a GameObject that has:
 * - visuals
 * - colliders
 * - a GameTerrain interface component
 */
[RequireComponent(typeof(IBiome))]
public class MeshTerrainGenerator : MonoBehaviour {

	public float Width = 100f;
	public float Length = 200f;

	public float MinimumHeight = 0f;
	public float MaximumHeight = 10f;

	public int ResolutionX = 8;
	public int ResolutionZ = 16;

	public Material GroundMaterial;

	public GameObject HeightNoise;
	INoise2D heightNoise;

	IBiome biomeGenerator;

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

	public GameObject Generate () {
		float[,] heightmap = generateHeightmap();

		// The space we must fill in between the vertices:
		int fillSize = (ResolutionZ-1) * (ResolutionX-1);

		Vector3[] vertices = new Vector3[ResolutionZ * ResolutionX];
		Color[] colors = new Color[ResolutionZ * ResolutionX];
		int[] triangles = new int[6 * fillSize];

		for (int z = 0; z < ResolutionZ; z++) {
			float zCoordinate = z * Length / (ResolutionZ-1);
			for (int x = 0; x < ResolutionX; x++) {
				float xCoordinate = x * Width / (ResolutionX-1);
				float yCoordinate = Mathf.Lerp(MinimumHeight, MaximumHeight, heightmap[z, x]);

				vertices[z * ResolutionX + x] = new Vector3(xCoordinate, yCoordinate, zCoordinate);
				// Simple vertex colors, can hook this up to a sort of biome system later for interesting terrain colors
				colors[z * ResolutionX + x] = biomeGenerator.GetGroundColor(new Vector2(xCoordinate, zCoordinate));
			}
		}

		int iTri = 0;
		for (int z = 0; z < ResolutionZ - 1; z++) {
			for (int x = 0; x < ResolutionX - 1; x++) {
				int iVert = z * ResolutionX + x;
				// First half of quad
				triangles[iTri + 0] = iVert;
				triangles[iTri + 1] = iVert + ResolutionX;
				triangles[iTri + 2] = iVert + 1;

				// Second half of quad
				triangles[iTri + 3] = iVert + ResolutionX;
				triangles[iTri + 4] = iVert + ResolutionX + 1;
				triangles[iTri + 5] = iVert + 1;

				iTri += 6;
			}
		}

		Mesh terrainMesh = new Mesh();
		terrainMesh.vertices = vertices;
		terrainMesh.triangles = triangles;
		terrainMesh.colors = colors;
		terrainMesh.RecalculateNormals();

		GameObject result = new GameObject("Generated Mesh Terrain");
		result.transform.position = new Vector3(-Width / 2f, MinimumHeight, 0f);

		MeshFilter meshFilter = result.AddComponent<MeshFilter>();
		meshFilter.mesh = terrainMesh;

		MeshRenderer meshRenderer = result.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = GroundMaterial;

		result.AddComponent<MeshCollider>();

		// Tie the Mesh into our game-side Terrain logic:
		result.AddComponent<MeshGameTerrain>();

		return result;
	}

	/**
	 * Generate the raw heightmap data
	 *
	 * Each point must lie between 0 and 1.
	 */
	float[,] generateHeightmap () {
		float[,] heightmap = new float[ResolutionZ, ResolutionX];

		for (int z = 0; z < ResolutionZ; z++) {
			float zCoordinate = z * Length / ResolutionZ;
			for (int x = 0; x < ResolutionX; x++) {
				float xCoordinate = x * Width / ResolutionX;
				heightmap[z, x] = heightNoise.Sample(new Vector2(xCoordinate, zCoordinate));
			}
		}

		return heightmap;
	}
}
