using UnityEngine;

public static class Chunk
{
	public const float WIDTH = 200f;
	public const float LENGTH = 200f;

	public static GridCoordinates groundPositionToGridCoordinates (Vector2 groundPosition) {
		return new GridCoordinates(
			Mathf.FloorToInt(groundPosition.x / Chunk.WIDTH),
			Mathf.FloorToInt(groundPosition.y / Chunk.LENGTH)
		);
	}

	public static Vector2 gridCoordinatesToGroundPosition (GridCoordinates coords) {
		return new Vector2(
			Chunk.WIDTH * coords.x,
			Chunk.LENGTH * coords.z
		);
	}

	public static Vector3 gridCoordinatesToWorldPosition (GridCoordinates coords) {
		return new Vector3(
			coords.x * Chunk.WIDTH,
			0.0f,
			coords.z * Chunk.LENGTH
		);
	}
}
