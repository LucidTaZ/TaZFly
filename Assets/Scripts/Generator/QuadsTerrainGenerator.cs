using UnityEngine;

/**
 * Generates a Terrain using a mesh
 * The distinctive feature is that each quad has a single color, instead of each vertex having its own color.
 *
 * The result is a GameObject that has:
 * - visuals
 * - colliders
 * - a GameTerrain interface component
 */
public class QuadsTerrainGenerator : TerrainGenerator {

	public float Width = 100f;
	public float Length = 200f;

	public float MinimumHeight = 0f;
	public float MaximumHeight = 10f;

	public int ResolutionX = 8;
	public int ResolutionZ = 16;

	public Material GroundMaterial;

	override protected GameObject Generate () {
		float[,] heightmap = GenerateHeightmap(ResolutionX, ResolutionZ, Width, Length);

		// The space we must fill in between the vertices:
		int fillSize = (ResolutionZ-1) * (ResolutionX-1);

		Vector3[] vertices = new Vector3[fillSize * 4];
		Color[] colors = new Color[fillSize * 4];
		int[] triangles = new int[6 * fillSize];

		int iTri = 0;
		for (int z = 0; z < ResolutionZ - 1; z++) {
			float z1 = z * Length / (ResolutionZ-1);
			float z2 = (z+1) * Length / (ResolutionZ-1);
			for (int x = 0; x < ResolutionX - 1; x++) {
				float x1 = x * Width / (ResolutionX-1);
				float x2 = (x+1) * Width / (ResolutionX-1);

				float y1 = Mathf.Lerp(MinimumHeight, MaximumHeight, heightmap[z, x]);
				float y2 = Mathf.Lerp(MinimumHeight, MaximumHeight, heightmap[z, (x+1)]);
				float y3 = Mathf.Lerp(MinimumHeight, MaximumHeight, heightmap[(z+1), (x+1)]);
				float y4 = Mathf.Lerp(MinimumHeight, MaximumHeight, heightmap[(z+1), x]);

				int i1 = z * (ResolutionX - 1) * 4 + 4 * x + 0;
				int i2 = z * (ResolutionX - 1) * 4 + 4 * x + 1;
				int i3 = z * (ResolutionX - 1) * 4 + 4 * x + 2;
				int i4 = z * (ResolutionX - 1) * 4 + 4 * x + 3;

				vertices[i1] = new Vector3(x1, y1, z1);
				vertices[i2] = new Vector3(x2, y2, z1);
				vertices[i3] = new Vector3(x2, y3, z2);
				vertices[i4] = new Vector3(x1, y4, z2);

				Color groundColor = biomeGenerator.GetGroundColor(new Vector2(x1, z1));
				colors[i1] = groundColor;
				colors[i2] = groundColor;
				colors[i3] = groundColor;
				colors[i4] = groundColor;

				// First half of quad
				triangles[iTri + 0] = i1;
				triangles[iTri + 1] = i3;
				triangles[iTri + 2] = i2;

				// Second half of quad
				triangles[iTri + 3] = i1;
				triangles[iTri + 4] = i4;
				triangles[iTri + 5] = i3;

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
}
