using UnityEngine;
using System.Collections;

public class PlayerController : ShipSteeringController {

	void Update () {
		Steer(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}
	
}
