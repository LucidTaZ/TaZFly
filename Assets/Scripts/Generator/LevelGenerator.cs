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

	public GameObject BoundaryPrefab;
	public GameObject FinishPrefab;

	public Vector3 PlayerSpawnPosition;
	public Vector3 EnemySpawnPosition;

	public GameObject Generate () {
		GameObject result = new GameObject("Generated Level");

		GameObject standards = generateStandards();
		standards.transform.parent = result.transform;

		GameObject terrain = GetComponent<TerrainGenerator>().Generate(2*Width, 1.4f * Length); // Generate the terrain larger than the playing area
		terrain.transform.parent = result.transform;

		List<GameObject> levelObjects = GetComponent<ObjectGenerator>().Generate(
			terrain.GetComponent<Terrain>(),
			new Rect(new Vector2(-Width / 2f, 0f), new Vector2(Width, Length))
		);
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
		playerSpawn.transform.position = PlayerSpawnPosition;
		playerSpawn.transform.parent = result.transform;
		
		GameObject enemy1Spawn = new GameObject("Enemy Spawn 1");
		enemy1Spawn.tag = "EnemySpawn";
		enemy1Spawn.transform.position = EnemySpawnPosition;
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

}
