using UnityEngine;

public class PerlinNoise2D : MonoBehaviour, INoise2D
{

	public int FrequencyScale = 15;
	public float BaselineImportance = 0.6f;

	/**
	 * Generate heightmap pixel
	 * 
	 * The output lies between 0 and 1
	 * 
	 * The coordinates are in world space.
	 */
	public float Sample(Vector2 point)
	{
		float baselineHeight = Mathf.PerlinNoise(point.y / FrequencyScale / 5 + 9999, 0f);
		float detailHeight = Mathf.PerlinNoise(point.x / FrequencyScale, point.y / FrequencyScale);
		return Mathf.Lerp(detailHeight, baselineHeight, BaselineImportance);
	}

}
