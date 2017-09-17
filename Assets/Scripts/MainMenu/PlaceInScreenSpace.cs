using UnityEngine;

public class PlaceInScreenSpace : MonoBehaviour {

	// Projects the screen coordinates onto the plane x=0, then places the GameObject there

	// [0..1]x[0..1]
	// (0,0) is bottom left
	public Vector2 ScreenCoordinates;

	// Use this for initialization
	void Start () {
		Vector3 screenCoordinatesWithDepth = new Vector3(
			ScreenCoordinates.x,
			ScreenCoordinates.y,
			getDistanceToCenterPlane()
		);
		Vector3 worldCoordinates = Camera.main.ViewportToWorldPoint(screenCoordinatesWithDepth);
		transform.position = worldCoordinates;
	}

	float getDistanceToCenterPlane () {
		Plane centerPlane = new Plane(Vector3.right, 0);

		Vector3 screenCoordinatesOnClippingPlane = new Vector3(
			ScreenCoordinates.x,
			ScreenCoordinates.y,
			Camera.main.nearClipPlane
		);
		Vector3 worldCoordinatesOnClippingPlane = Camera.main.ViewportToWorldPoint(screenCoordinatesOnClippingPlane);
		Vector3 direction = worldCoordinatesOnClippingPlane - Camera.main.transform.position;

		Ray placementRay = new Ray(Camera.main.transform.position, direction);

		float distance;
		centerPlane.Raycast(placementRay, out distance);
		return distance;
	}
}
