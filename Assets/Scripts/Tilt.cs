using UnityEngine;

public class Tilt : MonoBehaviour {

	public float Angle;

	void Start () {
		Vector2 direction = Random.insideUnitCircle;
		Vector3 axis = new Vector3(direction.x, 0f, direction.y);
		transform.Rotate(axis, Angle);
	}
}
