using UnityEngine;

/**
 * Generates a Terrain using Unity's Terrain component
 *
 * The result is a GameObject that has:
 * - visuals
 * - colliders
 * - a GameTerrain interface component
 */
public class UnityTerrainGenerator : TerrainGenerator {
	public float MinimumHeight = 0f;
	public float MaximumHeight = 10f;

	// Terrain textures:
	public Texture2D FlatTexture;
	public Texture2D SteepTexture;

	override protected void Awake () {
		base.Awake();
		if (ResolutionX != ResolutionZ) {
			Debug.LogWarning("Terrain X and Z resolution should be equal.");
		}
	}

	override protected GameObject Generate (Vector3 offset) {
		TerrainData terrainData = new TerrainData();
		terrainData.heightmapResolution = ResolutionX;
		terrainData.alphamapResolution = ResolutionX;
		
		Vector2 groundOffset = new Vector2(offset.x, offset.z);
		float[,] heightmap = GenerateHeightmap(groundOffset);
		terrainData.SetHeights(0, 0, heightmap);
		terrainData.size = new Vector3(Chunk.WIDTH, MaximumHeight - MinimumHeight, Chunk.LENGTH);
		
		GameObject result = Terrain.CreateTerrainGameObject(terrainData);
		result.transform.position = new Vector3(0f, MinimumHeight, 0f) + offset;
		result.GetComponent<Terrain>().Flush();

		// Tie the Unity Terrain into our game-side Terrain logic:
		result.AddComponent<UnityGameTerrain>();

		applyTextures(terrainData);

		return result;
	}
	
	void applyTextures (TerrainData terrainData) {
		SplatPrototype FlatSplat = new SplatPrototype();
		SplatPrototype SteepSplat = new SplatPrototype();
		
		FlatSplat.texture = FlatTexture;
		SteepSplat.texture = SteepTexture;
		
		terrainData.splatPrototypes = new SplatPrototype[]{FlatSplat, SteepSplat};
		terrainData.RefreshPrototypes();
		
		float[,,] splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 2];
		
		for (int y = 0; y < terrainData.alphamapHeight; y++) {
			for (int x = 0; x < terrainData.alphamapWidth; x++) {
				float normalizedX = (float)x / (terrainData.alphamapWidth - 1);
				float normalizedZ = (float)y / (terrainData.alphamapHeight - 1);
				
				float steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
				float steepnessNormalized = Mathf.Clamp(steepness / 25f, 0, 1f); // Division to tweak steepness to something that looks good
				
				splatMap[y, x, 0] = 1f - steepnessNormalized;
				splatMap[y, x, 1] = steepnessNormalized;
			}
		}
		
		terrainData.SetAlphamaps(0, 0, splatMap);
	}
}
