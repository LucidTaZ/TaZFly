using UnityEngine;

/**
 * Interface to a terrain object
 * This decouples terrain implementation from its use. For example, instead of working directly with a Unity Terrain
 * component, we may choose to generate a Mesh and facilitate the same functionality.
 */
public interface GameTerrain {
	Vector3 RaycastDownto (Vector2 coordinates, out bool hit);
}
