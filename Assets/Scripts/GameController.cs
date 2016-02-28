using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject ShipTemplate;

	GameObject instantiatedPlayerShip;
	List<GameObject> instantiatedEnemyShips = new List<GameObject>();

	void Start () {
		loadLevel();
	}

	public void PlayerFinished () {
		Debug.Log("Player finished.");
		Scene nextScene = SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1);
		GameObject gameFramework = GameObject.FindGameObjectWithTag("GameFramework");
		SceneManager.MoveGameObjectToScene(gameFramework, nextScene);
		SceneManager.LoadScene(nextScene.buildIndex);
	}

	void loadLevel () {
		Debug.Log("Loading level...");

		// Have origin
		if (GameObject.FindGameObjectsWithTag("Spawn").Length != 1) {
			throw new UnityException("Level must have 1 gameobject with tag Spawn.");
		}
		
		// Have finish
		if (GameObject.FindGameObjectsWithTag("BoundaryFinish").Length != 1) {
			throw new UnityException("Level must have 1 gameobject with tag BoundaryFinish.");
		}

		// Play in Z direction
		Vector3 playfield = GameObject.FindGameObjectWithTag("BoundaryFinish").transform.position - GameObject.FindGameObjectWithTag("Spawn").transform.position;
		if (playfield.z < playfield.x || playfield.z < playfield.y) {
			throw new UnityException("Please model the level as a box with the long side aligned with the Z axis.");
		}

		// Load Ship
		GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");
		instantiatePlayerShip(spawn.transform.position);

		// Load enemy Ships
		GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
		foreach (GameObject enemySpawn in enemySpawns) {
			instantiateEnemyShip(enemySpawn.transform.position);
		}
	}

	void instantiatePlayerShip (Vector3 spawnPoint) {
		instantiatedPlayerShip = instantiateShip(spawnPoint);
		attachPlayer(instantiatedPlayerShip);
	}

	void instantiateEnemyShip (Vector3 spawnPoint) {
		GameObject instantiatedShip = instantiateShip(spawnPoint);
		attachAI(instantiatedShip);
		instantiatedEnemyShips.Add(instantiatedShip);
	}

	GameObject instantiateShip (Vector3 spawnPoint) {
		GameObject ship = Instantiate(ShipTemplate) as GameObject;
		ship.transform.position = spawnPoint;
		return ship;
	}

	void attachPlayer (GameObject ship) {
		ShipSteeringController sc = ship.AddComponent<PlayerController>();
		sc.HorizontalMoveRate = 10;
		sc.VerticalMoveRate = 10;

		// Give player reference to cameras, so they know who to follow
		
		loadCameras(ship);

		// Give Player collider to Finish, so the Finish knows when to trigger
		// Idea: we can let the finishcontroller figure this out in Awake(), but only if we first revise the lifetime of
		// it, so that the player will be present at that time. One way to do that, I think, is to play every level in
		// its own scene.
		GameObject.FindGameObjectWithTag("BoundaryFinish").GetComponent<FinishController>().Load(instantiatedPlayerShip);
	}

	void attachAI (GameObject ship) {
		AIFactory aif = new AIFactory();
		float difficulty = Random.value;
		aif.AttachAI(ship, difficulty);
	}

	void loadCameras (GameObject ship) {
		// Give player reference to cameras, so they know who to follow
		
		Camera[] cameras = FindObjectsOfType<Camera>();
		foreach (Camera thisCamera in cameras) {
			CameraFollow cs;
			if ((cs = thisCamera.GetComponent<CameraFollow>()) != null) {
				cs.Load(ship);
			}
			CameraSwoon swoon;
			if ((swoon = thisCamera.GetComponent<CameraSwoon>()) != null) {
				swoon.Load(ship);
			}
			CameraFollow pcs;
			if ((pcs = thisCamera.GetComponentInParent<CameraFollow>()) != null) {
				pcs.Load(ship);
			}
			CameraSwoon pswoon;
			if ((pswoon = thisCamera.GetComponentInParent<CameraSwoon>()) != null) {
				pswoon.Load(ship);
			}
		}
	}
}
