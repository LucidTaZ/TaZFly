using UnityEngine;

public struct ChunkCreationContext
{
	public GridCoordinates Coordinates;
	public Rect GroundBoundary;

	public ChunkCreationContext (GridCoordinates coords, Rect groundBoundary) {
		Coordinates = coords;
		GroundBoundary = groundBoundary;
	}
}
