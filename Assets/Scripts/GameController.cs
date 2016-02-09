using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GUIController Gui;

	public GameObject ShipTemplate;
	public float MinY;
	public float MaxY;

	public GameObject[] Levels;

	GameObject instantiatedPlayerShip;
	List<GameObject> instantiatedEnemyShips = new List<GameObject>();
	List<GameObject> notifiedDeaths = new List<GameObject>();
	Hitpoints instantiatedPlayerHitpoints;
	
	List<IGameListener> listeners = new List<IGameListener>();

	int currentLevelIndex;
	GameObject currentLevel = null; // Points to the currently instantiated prefab

	bool loadNextLevelFlag; // Flag that steers loading the next level.
	bool levelUnloaded = false; // Flag that signals that the loading of a new level can start (Unity needs one game loop cycle to destroy stuff)
	bool levelActive = false; // Becomes true after level loading, becomes false before level unloading. Prevents null references after the last level has been played.

	public void Register (IGameListener listener) {
		listeners.Add(listener);
	}

	void Start () {
		listeners.ForEach(delegate(IGameListener listener) { listener.OnGameStarted(); });
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
			// See if we can generate a new one
			if (GameObject.FindWithTag("LevelGenerator")) {
				LevelGenerator generator = GameObject.FindWithTag("LevelGenerator").GetComponent<LevelGenerator>();
				GameObject level = generator.Generate();
				currentLevel = loadLevel(level);
			} else {
				throw new UnityException("Out of levels.");
			}
		} else {
			currentLevel = loadLevel(Instantiate(Levels[currentLevelIndex])); // currentLevel becomes a reference to the instantiation
		}

		levelActive = true;
		listeners.ForEach(delegate(IGameListener listener) { listener.OnLevelStarted(currentLevel); });
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

		if (Input.GetKey(KeyCode.Escape)) {
			Debug.Log("Player quit.");
			listeners.ForEach(delegate(IGameListener listener) { listener.OnLevelEnded(currentLevel); listener.OnGameEnded(); });
			Application.Quit();
		}

		checkPlayers();
	}

	public void PlayerFinished () {
		//Debug.Log("Player finished.");
		listeners.ForEach(delegate(IGameListener listener) { listener.OnPlayerFinished(instantiatedPlayerShip); }); // TODO: Fire event also for enemy finished
		loadNextLevelFlag = true;
	}

	void unloadLevel () {
		Debug.Log("Unloading level...");

		levelActive = false;
		listeners.ForEach(delegate(IGameListener listener) { listener.OnLevelEnded(currentLevel); });
		if (currentLevel != null) {
			unloadCameras();

			DestroyObject(currentLevel);

			if (instantiatedPlayerShip != null) {
				DestroyObject(instantiatedPlayerShip);
			}
			instantiatedEnemyShips.ForEach(delegate(GameObject enemyShip) {
				if (enemyShip != null) {
					DestroyObject(enemyShip);
				}
			});
			instantiatedEnemyShips.Clear();
			notifiedDeaths.Clear();

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
		
		// Have boundary
		if (GameObject.FindGameObjectsWithTag("BoundaryFinish").Length != 1) {
			throw new UnityException("Level must have 1 gameobject with tag BoundaryFinish.");
		}
		
		if (GameObject.FindGameObjectsWithTag("BoundaryLeft").Length != 1) {
			throw new UnityException("Level must have 1 gameobject with tag BoundaryLeft.");
		}
		
		if (GameObject.FindGameObjectsWithTag("BoundaryRight").Length != 1) {
			throw new UnityException("Level must have 1 gameobject with tag BoundaryRight.");
		}

		// Play in Z direction
		Vector3 playfield = GameObject.FindGameObjectWithTag("BoundaryFinish").transform.position - GameObject.FindGameObjectWithTag("Spawn").transform.position;
		if (playfield.z < playfield.x || playfield.z < playfield.y) {
			throw new UnityException("Please model the level as a box with the long side aligned with the Z axis.");
		}

		Boundary fieldBoundary = new Boundary();
		fieldBoundary.MinX = GameObject.FindGameObjectWithTag("BoundaryLeft").transform.position.x;
		fieldBoundary.MaxX = GameObject.FindGameObjectWithTag("BoundaryRight").transform.position.x;
		fieldBoundary.MinY = MinY;
		fieldBoundary.MaxY = MaxY;

		// Load Ship
		GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");
		instantiatePlayerShip(fieldBoundary, level, spawn.transform.position);

		// Load enemy Ships
		GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
		foreach (GameObject enemySpawn in enemySpawns) {
			instantiateEnemyShip(fieldBoundary, level, enemySpawn.transform.position);
		}

		return level;
	}

	void instantiatePlayerShip (Boundary fieldBoundary, GameObject level, Vector3 spawnPoint) {
		instantiatedPlayerShip = instantiateShip(fieldBoundary, level, spawnPoint);
		attachPlayer(instantiatedPlayerShip);
		listeners.ForEach(delegate(IGameListener listener) { listener.OnPlayerCreated(instantiatedPlayerShip); });
	}

	void instantiateEnemyShip (Boundary fieldBoundary, GameObject level, Vector3 spawnPoint) {
		GameObject instantiatedShip = instantiateShip(fieldBoundary, level, spawnPoint);
		attachAI(instantiatedShip);
		instantiatedEnemyShips.Add(instantiatedShip);
		listeners.ForEach(delegate(IGameListener listener) { listener.OnPlayerCreated(instantiatedShip); });
	}

	GameObject instantiateShip (Boundary fieldBoundary, GameObject level, Vector3 spawnPoint) {
		GameObject ship = Instantiate(ShipTemplate) as GameObject;
		ship.transform.parent = level.transform;
		ship.transform.position = spawnPoint;

		ShipController shipController = ship.GetComponent<ShipController>();
		shipController.Load(fieldBoundary);
		return ship;
	}

	void attachPlayer (GameObject ship) {
		instantiatedPlayerHitpoints = ship.GetComponent<Hitpoints>();
		Gui.SetPlayer(ship);

		ShipSteeringController sc = ship.AddComponent<PlayerController>();
		sc.HorizontalMoveRate = 10;
		sc.VerticalMoveRate = 10;

		// Give player reference to cameras, so they know who to follow
		
		loadCameras(ship);

		// Give Player collider to Finish, so the Finish knows when to trigger
		
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

	/**
	 * See who is still alive and send events for those that died
	 */
	void checkPlayers () {
		if (instantiatedPlayerShip == null) {
			loadNextLevelFlag = true;
		}
		if (instantiatedPlayerShip != null && !instantiatedPlayerShip.GetComponent<Hitpoints>().IsAlive()) {
			if (!notifiedDeaths.Contains(instantiatedPlayerShip)) {
				listeners.ForEach(delegate(IGameListener listener) { listener.OnPlayerDestroyed(instantiatedPlayerShip); });
				notifiedDeaths.Add(instantiatedPlayerShip);
			}
		}
		for (int i = 0; i < instantiatedEnemyShips.Count; i++) {
			GameObject enemy = instantiatedEnemyShips[i];
			if (enemy == null || !enemy.GetComponent<Hitpoints>().IsAlive()) {
				if (!notifiedDeaths.Contains(enemy)) {
					listeners.ForEach(delegate(IGameListener listener) { listener.OnPlayerDestroyed(enemy); });
					instantiatedEnemyShips.RemoveAt(i); // Lose track of ship, the ship will destroy itself via a timer
					notifiedDeaths.Add(enemy);
				}
			}
		}
	}
}
