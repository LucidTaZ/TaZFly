using UnityEngine;

public class PerlinNoise2D : MonoBehaviour, INoise2D
{
	public int FrequencyScale = 15; // Higher = smoother, bigger terrain
	public float BaselineImportance = 0.6f;
	public float Skew = 1.0f; // Over 1 = stretch lengthwise, under 1 = stretch broadwise. Used to give the player some advantage of flying into a good spot, by allowing him to stay there longer

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
		float detailHeight = Mathf.PerlinNoise(point.x * Skew / FrequencyScale, point.y / Skew / FrequencyScale);
		return Mathf.Lerp(detailHeight, baselineHeight, BaselineImportance);
	}

}
