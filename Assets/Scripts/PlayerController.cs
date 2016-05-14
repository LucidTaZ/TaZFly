using UnityEngine;

public class PlayerController : ShipSteeringController {

	void Update () {
		Steer(Input.GetAxis("Horizontal"), Input.GetAxis("VerticalPlane"));
	}
	
}
