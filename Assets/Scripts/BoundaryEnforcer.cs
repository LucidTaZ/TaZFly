using UnityEngine;

public class BoundaryEnforcer : MonoBehaviour {

	public Boundary FieldBoundary;

	void Update () {
		GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");
		foreach (GameObject ship in ships) {
			ship.transform.position = clampPosition(ship.transform.position, FieldBoundary);
		}
	}

	public bool IsInVerticalBounds (Vector3 position) {
		if (!enabled) {
			return true;
		}
		return position.y > FieldBoundary.MinY && position.y < FieldBoundary.MaxY;
	}

	public bool IsInHorizontalBounds (Vector3 position) {
		if (!enabled) {
			return true;
		}
		return position.x > FieldBoundary.MinX && position.x < FieldBoundary.MaxX;
	}

	static Vector3 clampPosition (Vector3 position, Boundary fieldBoundary) {
		return new Vector3(
			Mathf.Clamp(position.x, fieldBoundary.MinX, fieldBoundary.MaxX),
			Mathf.Clamp(position.y, fieldBoundary.MinY, fieldBoundary.MaxY),
			position.z
		);
	}
}
