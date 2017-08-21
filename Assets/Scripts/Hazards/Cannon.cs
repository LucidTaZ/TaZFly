using UnityEngine;

public class Cannon : MonoBehaviour {

	public GameObject Projectile;
	public Transform EjectionPoint;
	public float ProjectileSpeed = 20;
	public float ShotInterval = 3;
	public float ProjectileSpeedVar = 0;
	public float ShotIntervalVar = 0;

	float LastShot;

	void Awake () {
		LastShot = 0;
		ProjectileSpeed += (Random.value*2-1) * ProjectileSpeedVar;
		ShotInterval += (Random.value*2-1) * ShotIntervalVar;
	}

	void LateUpdate () {
		if (Time.time > LastShot + ShotInterval) {
			Shoot();
		}
	}

	void Shoot () {
		LastShot = Time.time;

		GameObject projectile = (GameObject)Instantiate(Projectile);
		projectile.transform.parent = gameObject.transform; // Cannonball is a child of the cannon. Makes sure it gets unloaded properly when the cannon is unloaded.
		projectile.transform.position = EjectionPoint.position;
		projectile.transform.rotation = transform.rotation;
		projectile.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value, Random.value, Random.value) * 360);
		projectile.GetComponent<Rigidbody>().velocity = transform.TransformVector(Vector3.up * ProjectileSpeed);
	}

}
