using UnityEngine;
using System.Collections.Generic;

/**
 * Script to generate random levels
 * 
 * Usage
 * 
 * Have a GameObject in the scene with this script attached and with the "LevelGenerator" tag. When the GameController
 * runs out of levels, it will generate new ones using this script.
 */
public class LevelGenerator : MonoBehaviour {

	public float Width = 50f;
	public float Length = 150f;
	public float MinimumTerrainHeight = 0f;
	public float MaximumTerrainHeight = 10f;

	public GameObject BoundaryPrefab;
	public GameObject FinishPrefab;

	// Terrain textures:
	public Texture2D FlatTexture;
	public Texture2D SteepTexture;

	public GameObject Generate () {
		GameObject result = new GameObject("Generated Level");

		GameObject standards = generateStandards();
		standards.transform.parent = result.transform;

		GameObject terrain = generateTerrain(Width, Length, MinimumTerrainHeight, MaximumTerrainHeight);
		terrain.transform.parent = result.transform;

		List<GameObject> levelObjects = generateObjects();
		foreach (GameObject levelObject in levelObjects) {
			levelObject.transform.parent = result.transform;
		}

		return result;
	}

	/**
	 * Generate standard stuff that is present in every level
	 * 
	 * - Player spawn
	 * - Enemy spawns
	 * - Boundary (left, right, finish)
	 * - Sunlight
	 */
	GameObject generateStandards () {
		GameObject result = new GameObject("Standard");

		GameObject playerSpawn = new GameObject("Spawn");
		playerSpawn.tag = "Spawn";
		playerSpawn.transform.position = new Vector3(6f, 10f, 0f);
		playerSpawn.transform.parent = result.transform;
		
		GameObject enemy1Spawn = new GameObject("Enemy Spawn 1");
		enemy1Spawn.tag = "EnemySpawn";
		enemy1Spawn.transform.position = new Vector3(8f, 10f, 0f);
		enemy1Spawn.transform.parent = result.transform;

		GameObject boundary = new GameObject("Boundary");
		boundary.transform.parent = result.transform;

		GameObject boundaryLeft = Instantiate(BoundaryPrefab);
		boundaryLeft.tag = "BoundaryLeft";
		boundaryLeft.transform.position = new Vector3(-Width / 2f, 10f, Length / 2f);
		boundaryLeft.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
		boundaryLeft.transform.localScale = new Vector3(Width / 10f, 1f, Length / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		boundaryLeft.transform.parent = boundary.transform;
		
		GameObject boundaryRight = Instantiate(BoundaryPrefab);
		boundaryRight.tag = "BoundaryRight";
		boundaryRight.transform.position = new Vector3(Width / 2f, 10f, Length / 2f);
		boundaryRight.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		boundaryRight.transform.localScale = new Vector3(Width / 10f, 1f, Length / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		boundaryRight.transform.parent = boundary.transform;

		GameObject boundaryFinish = Instantiate(FinishPrefab);
		boundaryFinish.tag = "BoundaryFinish";
		boundaryFinish.transform.position = new Vector3(0f, 10f, Length);
		boundaryFinish.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
		boundaryFinish.transform.localScale = new Vector3(Width / 10f, 1f, Width / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		boundaryFinish.transform.parent = boundary.transform;

		GameObject sunlight = new GameObject("Sunlight");
		sunlight.transform.rotation = Quaternion.Euler(60f, 30f, 0f);
		Light lightSource = sunlight.AddComponent<Light>();
		lightSource.type = LightType.Directional;
		lightSource.shadows = LightShadows.Soft;
		lightSource.intensity = 1.58f;
		lightSource.color = Color.white;
		sunlight.transform.parent = result.transform;
		GameObject.FindWithTag("LevelGenerator");
		return result;
	}

	/**
	 * Generate the GameObject that contains the terrain
	 */
	GameObject generateTerrain (float width, float length, float minHeight, float maxHeight) {
		int resolution = 128;

		TerrainData terrainData = new TerrainData();
		terrainData.heightmapResolution = resolution;
		terrainData.alphamapResolution = resolution;
		
		float[,] heightmap = generateHeightmap(resolution, width, length);
		terrainData.SetHeights(0, 0, heightmap);
		terrainData.size = new Vector3(width, maxHeight - minHeight, length);
		
		GameObject result = Terrain.CreateTerrainGameObject(terrainData);
		result.transform.position = new Vector3(-width / 2f, minHeight, 0f);
		result.GetComponent<Terrain>().Flush();

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

	/**
	 * Generate the objects that are present (cannons, barrels etc)
	 */
	List<GameObject> generateObjects () {
		return new List<GameObject>();
	}

}
