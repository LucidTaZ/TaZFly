using UnityEngine;

public class Cannon : MonoBehaviour {

	public GameObject Projectile;
	public Transform EjectionPoint;
	public float ProjectileSpeed;
	public float ShotInterval;
	float LastShot;

	void Start () {
		LastShot = 0;
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
