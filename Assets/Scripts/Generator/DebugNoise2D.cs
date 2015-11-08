using UnityEngine;

public class DebugNoise2D : MonoBehaviour, INoise2D
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
		return (Mathf.Sin(point.x / 10) * Mathf.Sin(point.y / 15) + 1) / 2;
	}

}
