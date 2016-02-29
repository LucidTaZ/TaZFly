using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GUIController Gui;

	public GameObject ShipTemplate;

	public GameObject[] Levels;

	int currentLevelIndex;
	GameObject currentLevel = null; // Points to the currently instantiated prefab

	bool loadNextLevelFlag; // Flag that steers loading the next level.
	bool levelUnloaded = false; // Flag that signals that the loading of a new level can start (Unity needs one game loop cycle to destroy stuff)

	void Start () {
		loadNextLevel(0);
	}

	void loadNextLevel (int setLevelIndex = -1) {
		Debug.Log("Loading next level.");

		if (setLevelIndex >= 0) {
			currentLevelIndex = setLevelIndex;
		} else {
			++currentLevelIndex;
		}

		if (currentLevelIndex >= Levels.Length) {
			throw new UnityException("Out of levels.");
		} else {
			currentLevel = loadLevel(Instantiate(Levels[currentLevelIndex])); // currentLevel becomes a reference to the instantiation
		}
	}

	void Update () {
		//Debug.Log("Update()...");
		if (loadNextLevelFlag) {
			if (!levelUnloaded) {
				unloadLevel();
			} else {
				try {
					loadNextLevel();
				} catch (UnityException e) {
					Debug.Log(e.Message);
					Application.Quit();
				}
				loadNextLevelFlag = false;
				levelUnloaded = false;
			}
		}
	}

	public void PlayerFinished () {
		//Debug.Log("Player finished.");
		loadNextLevelFlag = true;
	}

	void unloadLevel () {
		Debug.Log("Unloading level...");

		if (currentLevel != null) {
			unloadCameras();

			DestroyObject(currentLevel);

			currentLevel = null;
		}

		// Really check whether Unity did its job:
		if (GameObject.FindGameObjectsWithTag("Spawn").Length == 0) {
			levelUnloaded = true;
		}
	}

	GameObject loadLevel (GameObject level) {
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
		instantiatePlayerShip(level, spawn.transform.position);

		// Load enemy Ships
		GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
		foreach (GameObject enemySpawn in enemySpawns) {
			instantiateEnemyShip(level, enemySpawn.transform.position);
		}

		return level;
	}

	void instantiatePlayerShip (GameObject level, Vector3 spawnPoint) {
		GameObject instantiatedPlayerShip = instantiateShip(level, spawnPoint);
		attachPlayer(instantiatedPlayerShip);
	}

	void instantiateEnemyShip (GameObject level, Vector3 spawnPoint) {
		GameObject instantiatedShip = instantiateShip(level, spawnPoint);
		attachAI(instantiatedShip);
	}

	GameObject instantiateShip (GameObject level, Vector3 spawnPoint) {
		GameObject ship = Instantiate(ShipTemplate) as GameObject;
		ship.transform.parent = level.transform;
		ship.transform.position = spawnPoint;
		return ship;
	}

	void attachPlayer (GameObject ship) {
		Gui.SetPlayer(ship);

		ShipSteeringController sc = ship.AddComponent<PlayerController>();
		sc.HorizontalMoveRate = 10;
		sc.VerticalMoveRate = 10;

		// Give player reference to cameras, so they know who to follow
		
		loadCameras(ship);

		// Give Player collider to Finish, so the Finish knows when to trigger
		// Idea: we can let the finishcontroller figure this out in Awake(), but only if we first revise the lifetime of
		// it, so that the player will be present at that time. One way to do that, I think, is to play every level in
		// its own scene.
		GameObject.FindGameObjectWithTag("BoundaryFinish").GetComponent<FinishController>().Load(ship);
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

	void unloadCameras () {
		Camera[] cameras = FindObjectsOfType<Camera>();
		foreach (Camera thisCamera in cameras) {
			CameraFollow cs;
			if ((cs = thisCamera.GetComponent<CameraFollow>()) != null) {
				cs.Unload();
			}
			CameraSwoon swoon;
			if ((swoon = thisCamera.GetComponent<CameraSwoon>()) != null) {
				swoon.Unload();
			}
		}
	}
}
