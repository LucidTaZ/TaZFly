using UnityEngine;

public class DebugDisplay : MonoBehaviour, IGameListener {
	public void Start () {
		GetComponent<GameController>().Register(this);
		Debug.Log("Registered to GameController events.");
	}
	
	public void OnGameStarted () {
		Debug.Log("Game started.");
	}
	
	public void OnGameEnded () {
		Debug.Log("Game ended.");
	}
	
	public void OnLevelStarted (GameObject level) {
		Debug.Log("Level started.");
	}
	
	public void OnLevelEnded (GameObject level) {
		Debug.Log("Level ended.");
	}
	
	public void OnPlayerCreated (GameObject player) {
		Debug.Log("Player created.");
	}
	
	public void OnPlayerDestroyed (GameObject player) {
		Debug.Log("Player destroyed.");
	}
	
	public void OnPlayerFinished (GameObject player) {
		Debug.Log("Player finished.");
	}
}
