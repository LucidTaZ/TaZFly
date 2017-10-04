using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	public float Width = 50f;
	public float Length = 150f;

	public float SlowHeight = 25f;
	public float SpeedyHeight = 3f;

	public int EnemyCount = 1;

	public GameObject FinishPrefab;

	public Vector3 PlayerSpawnPosition = new Vector3(0f, 20f, 0f);
	public Vector3 EnemySpawnPosition = new Vector3(0f, 15f, 0f);

	public GameObject SunPrefab;

	public void Awake () {
		GameObject level = Generate();
		level.transform.parent = gameObject.transform;
	}

	public GameObject Generate () {
		GameObject result = new GameObject("Generated Level");

		GameObject standards = generateStandards();
		standards.transform.parent = result.transform;

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
			const float enemySpacing = 4f;
			float enemyOffset = -EnemyCount/2 + i;
			enemySpawn.transform.position = EnemySpawnPosition + new Vector3(enemySpacing * enemyOffset, 0, 0);
			enemySpawn.transform.parent = result.transform;
		}

		GameObject slowHeightMarker = new GameObject("SlowHeight");
		slowHeightMarker.tag = "SlowHeight";
		slowHeightMarker.transform.position = new Vector3(0f, SlowHeight, 0f);
		slowHeightMarker.transform.parent = result.transform;

		GameObject speedyHeightMarker = new GameObject("SpeedyHeight");
		speedyHeightMarker.tag = "SpeedyHeight";
		speedyHeightMarker.transform.position = new Vector3(0f, SpeedyHeight, 0f);
		speedyHeightMarker.transform.parent = result.transform;

		GameObject boundary = new GameObject("Boundary");
		boundary.transform.parent = result.transform;

		GameObject boundaryLeftMarker = new GameObject("BoundaryLeft");
		boundaryLeftMarker.tag = "BoundaryLeft";
		boundaryLeftMarker.transform.position = new Vector3(-Width / 2, SlowHeight, 0f);
		boundaryLeftMarker.transform.parent = result.transform;

		GameObject boundaryRightMarker = new GameObject("BoundaryRight");
		boundaryRightMarker.tag = "BoundaryRight";
		boundaryRightMarker.transform.position = new Vector3(Width / 2, SlowHeight, 0f);
		boundaryRightMarker.transform.parent = result.transform;

		if (Length < Mathf.Infinity) {
			GameObject boundaryFinish = Instantiate(FinishPrefab);
			boundaryFinish.transform.position = new Vector3(0f, SpeedyHeight, Length);
			boundaryFinish.transform.parent = boundary.transform;
		}

		GameObject sunlight = Instantiate(SunPrefab);
		sunlight.transform.parent = result.transform;

		return result;
	}
}
