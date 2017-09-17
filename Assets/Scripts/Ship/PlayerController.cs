using UnityEngine;

public class PlayerController : ShipSteeringController {
	void Update () {
		SteerLocalSpace(Input.GetAxis("Horizontal"), Input.GetAxis("VerticalPlane"));
	}
}
