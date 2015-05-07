using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GUIText HitpointText;
	public GUIText SpeedText;

	public GameObject ShipTemplate;
	protected GameObject InstantiatedPlayerShip;
	protected ShipController InstantiatedPlayerShipController;

	public float MinY;
	public float MaxY;

	public GameObject[] Levels;
	private int currentLevelIndex;
	private GameObject currentLevel = null; // Points to the currently instantiated prefab

	private bool loadNextLevelFlag; // Flag that steers loading the next level.
	private bool levelUnloaded = false; // Flag that signals that the loading of a new level can start (Unity needs one game loop cycle to destroy stuff)
	private bool levelActive = false; // Becomes true after level loading, becomes false before level unloading. Prevents null references after the last level has been played.

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
		}

		currentLevel = loadLevel(Levels[currentLevelIndex]); // currentLevel becomes a reference to the instantiation
		levelActive = true;
	}

	void Update () {
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
			Application.Quit();
		}

		if (levelActive) {
			if (!InstantiatedPlayerShipController.IsAlive()) {
				//Debug.Log("Player dead.");
				Application.Quit();
			}

			HitpointText.text = System.String.Format("Hitpoints: {0:P0}", Mathf.Max(0, InstantiatedPlayerShipController.GetRelativeHitpoints()));
			SpeedText.text = System.String.Format("Speed: {0:N2}", InstantiatedPlayerShip.GetComponent<Rigidbody>().velocity.z);
		}
	}

	public void PlayerFinished () {
		//Debug.Log("Player finished.");
		loadNextLevelFlag = true;
	}

	void unloadLevel () {
		levelActive = false;
		if (currentLevel != null) {
			unloadCameras();
			//Debug.Log("Destroying current level with name: " + currentLevel.name);
			DestroyObject(currentLevel);
			//DestroyImmediate(currentLevel);
			currentLevel = null;
		}

		// Really check whether Unity did its job:
		if (GameObject.FindGameObjectsWithTag("Spawn").Length == 0) {
			levelUnloaded = true;
		}
	}

	GameObject loadLevel (GameObject levelTemplate) {
		GameObject level = Instantiate(levelTemplate) as GameObject;
		
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
		InstantiatedPlayerShip = instantiateShip(fieldBoundary, level, spawnPoint);
		attachPlayer(InstantiatedPlayerShip);
	}

	void instantiateEnemyShip (Boundary fieldBoundary, GameObject level, Vector3 spawnPoint) {
		GameObject instantiatedShip = instantiateShip(fieldBoundary, level, spawnPoint);
		attachAI(instantiatedShip);
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
		InstantiatedPlayerShipController = ship.GetComponent<ShipController>();

		ShipSteeringController sc = ship.AddComponent<PlayerController>();
		sc.HorizontalMoveRate = 8;
		sc.VerticalMoveRate = 8;

		// Give player reference to cameras, so they know who to follow
		
		loadCameras(ship);

		// Give Player collider to Finish, so the Finish knows when to trigger
		
		GameObject.FindGameObjectWithTag("BoundaryFinish").GetComponent<FinishController>().Load(InstantiatedPlayerShip);
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
			CameraController cs;
			if ((cs = thisCamera.GetComponent<CameraController>()) != null) {
				cs.Load(ship);
			}
		}
	}

	void unloadCameras () {
		Camera[] cameras = FindObjectsOfType<Camera>();
		foreach (Camera thisCamera in cameras) {
			CameraController cs;
			if ((cs = thisCamera.GetComponent<CameraController>()) != null) {
				cs.Unload();
			}
		}
	}

}
