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

	// Boundary vertical placement
	// The Boundary will range from -Height/2+OffsetHeight to Height/2+OffsetHeight
	// So for values Height 50 and OffsetHeight 10, the result is -15 to 35.
	// To make these nicely settable via the editor, we should make it more intuitive first.
	const float Height = 50f; // Of the boundary
	const float OffsetHeight = 10f; // How high to place the middle of the boundary?

	public int EnemyCount = 1;

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

		for (int i = 1; i <= EnemyCount; i++) {
			GameObject enemySpawn = new GameObject("Enemy Spawn " + i);
			enemySpawn.tag = "EnemySpawn";
			float enemySpacing = 4f;
			float enemyOffset = -EnemyCount/2 + i;
			enemySpawn.transform.position = EnemySpawnPosition + new Vector3(enemySpacing * enemyOffset, 0, 0);
			enemySpawn.transform.parent = result.transform;
		}

		GameObject boundary = new GameObject("Boundary");
		boundary.transform.parent = result.transform;

		GameObject boundaryLeft = Instantiate(BoundaryPrefab);
		boundaryLeft.tag = "BoundaryLeft";
		boundaryLeft.transform.position = new Vector3(-Width / 2f, OffsetHeight, Length / 2f);
		boundaryLeft.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
		boundaryLeft.transform.localScale = new Vector3(Height / 10f, 1f, Length / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		fixTextureScale(boundaryLeft);
		boundaryLeft.transform.parent = boundary.transform;

		GameObject boundaryRight = Instantiate(BoundaryPrefab);
		boundaryRight.tag = "BoundaryRight";
		boundaryRight.transform.position = new Vector3(Width / 2f, OffsetHeight, Length / 2f);
		boundaryRight.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		boundaryRight.transform.localScale = new Vector3(Height / 10f, 1f, Length / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		fixTextureScale(boundaryRight);
		boundaryRight.transform.parent = boundary.transform;

		GameObject boundaryTop = Instantiate(BoundaryPrefab);
		boundaryTop.tag = "BoundaryTop";
		boundaryTop.transform.position = new Vector3(0, OffsetHeight + Height / 2, Length / 2f);
		boundaryTop.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
		boundaryTop.transform.localScale = new Vector3(Width / 10f, 1f, Length / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		boundaryTop.GetComponent<MeshRenderer>().enabled = false;
		fixTextureScale(boundaryTop);
		boundaryTop.transform.parent = boundary.transform;

		GameObject boundaryBottom = Instantiate(BoundaryPrefab);
		boundaryBottom.tag = "BoundaryBottom";
		boundaryBottom.transform.position = new Vector3(0, 0f, Length / 2f);
		boundaryBottom.transform.rotation = Quaternion.Euler(0f, 0f, 00f);
		boundaryBottom.transform.localScale = new Vector3(Width / 10f, 1f, Length / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		boundaryBottom.GetComponent<MeshRenderer>().enabled = false;
		fixTextureScale(boundaryBottom);
		boundaryBottom.transform.parent = boundary.transform;

		GameObject boundaryFinish = Instantiate(FinishPrefab);
		boundaryFinish.tag = "BoundaryFinish";
		boundaryFinish.transform.position = new Vector3(0f, OffsetHeight, Length);
		boundaryFinish.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
		boundaryFinish.transform.localScale = new Vector3(Width / 10f, 1f, Height / 10f); // Divide by 10 since the Plane has dimensions 10x10 already.
		fixTextureScale(boundaryFinish);
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

	private void fixTextureScale(GameObject subject) {
		// Make the texture tile well even as we stretch the quad
		subject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(subject.transform.localScale.x, subject.transform.localScale.z);
	}
}
