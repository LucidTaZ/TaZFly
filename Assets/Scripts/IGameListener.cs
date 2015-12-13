using UnityEngine;

public interface IGameListener
{
	void OnGameStarted();
	void OnGameEnded();

	void OnLevelStarted(GameObject level);
	void OnLevelEnded(GameObject level);

	void OnPlayerCreated(GameObject player);
	void OnPlayerDestroyed(GameObject player);
	void OnPlayerFinished(GameObject player);
}
