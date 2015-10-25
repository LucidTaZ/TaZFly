using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

	public int MapResolution = 128;

	public float MinimumHeight = 0f;
	public float MaximumHeight = 10f;

	// Terrain textures:
	public Texture2D FlatTexture;
	public Texture2D SteepTexture;

	public GameObject Generate (float width, float length) {
		TerrainData terrainData = new TerrainData();
		terrainData.heightmapResolution = MapResolution;
		terrainData.alphamapResolution = MapResolution;
		
		float[,] heightmap = generateHeightmap(MapResolution, width, length);
		terrainData.SetHeights(0, 0, heightmap);
		terrainData.size = new Vector3(width, MaximumHeight - MinimumHeight, length);
		
		GameObject result = Terrain.CreateTerrainGameObject(terrainData);
		result.transform.position = new Vector3(-width / 2f, MinimumHeight, 0f);
		result.GetComponent<Terrain>().Flush();
		
		applyTextures(terrainData);
		
		return result;
	}
	
	/**
	 * Generate the GameObject that contains the terrain
	 */
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
	
	/**
	 * Generate the raw heightmap data
	 * 
	 * Each point must lie between 0 and 1.
	 * 
	 * Parameters:
	 * - resolution: the resolution of the (sqare) height map
	 * - width: width of the actual terrain that is being created
	 * - height: height of the actual terrain that is being created
	 * 
	 * The width and height parameters are to know the shape of the (not necessarily square) terrain.
	 */
	static float[,] generateHeightmap (int resolution, float width, float height) {
		float[,] heightmap = new float[resolution, resolution];
		
		for (int y = 0; y < resolution; y++) {
			float yCoordinate = y * height / resolution;
			for (int x = 0; x < resolution; x++) {
				float xCoordinate = x * width / resolution;
				heightmap[y, x] = generateHeightAtPoint(new Vector2(xCoordinate, yCoordinate));
			}
		}
		
		return heightmap;
	}
	
	/**
	 * Generate heightmap pixel
	 * 
	 * The output lies between 0 and 1
	 * 
	 * The coordinates are in world space.
	 */
	static float generateHeightAtPoint (Vector2 point) {
		return (Mathf.Sin(point.x / 10) * Mathf.Sin(point.y / 15) + 1) / 2;
	}
}
