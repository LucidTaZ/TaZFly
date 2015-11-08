public interface INoise2D
{

	/**
	 * Generate heightmap pixel
	 * 
	 * The output lies between 0 and 1
	 * 
	 * The coordinates are in world space.
	 */
	float Sample(UnityEngine.Vector2 point);

}
