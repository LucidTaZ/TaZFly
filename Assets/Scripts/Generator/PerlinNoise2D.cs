using UnityEngine;

public class PerlinNoise2D : MonoBehaviour, INoise2D
{

	/**
	 * Generate heightmap pixel
	 * 
	 * The output lies between 0 and 1
	 * 
	 * The coordinates are in world space.
	 */
	public float Sample(Vector2 point)
	{
		return 0.0f; // TODO: Implement
	}

}
