using UnityEngine;

public interface IBiome
{
	void Initialize();
	Color GetGroundColor(Vector2 position);
}

