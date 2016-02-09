using UnityEngine;

public class Hitpoints : MonoBehaviour, IHitpointsUser {

    public int HitpointsValue;
	public float DamagePerNewton = 1000;

	public GameObject DeathExplosion;

	protected bool exploded = false;

	protected HitpointsController controller;

    void OnEnable()
    {
		controller.SetHitpointsUser(this);
    }

	void Awake()
	{
		controller = new HitpointsController(HitpointsValue); // Take over setting from editor.
	}

    void OnCollisionEnter(Collision collision)
    {
		if (!IsAlive()) {
			// We are in free fall mode after hitpoints depletion. Anything we hit will detonate us.
			Explode();
		}
        if (collision.collider.GetComponent<OverrideDamage>()) {
            controller.Decrease(collision.collider.GetComponent<OverrideDamage>().Damage);
        } else {
			//controller.Decrease((int)collision.relativeVelocity.magnitude);
			float force = collision.impulse.magnitude / Time.fixedDeltaTime;
			controller.Decrease((int)(force / DamagePerNewton));
        }
    }

	public void Die ()
	{
		if (GetComponent<PlayerController>()) {
			Debug.Log("Starting to die...");
		}
		Destroy(GetComponent<AutomaticSpeed>());
		Destroy(GetComponent<ShipSteeringController>());
		Destroy(GetComponent<BankByVelocity>());
		GetComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().freezeRotation = false;
		GetComponent<PropellerController>().StopSpinning();
		if (GetComponent<PlayerController>()) {
			Debug.Log("Died.");
		}
	}

	void Explode()
	{
		// Explode the Detonator framework explosion
		if (!exploded) {
			if (DeathExplosion) {
				GameObject detonatorObject = GameObject.Instantiate(DeathExplosion);
				DeathExplosion = null; // Make sure it only happens once!
				detonatorObject.transform.position = transform.position;
				detonatorObject.GetComponent<Detonator>().Explode();
				CameraShakePositional.ShakeAtLocation(transform.position, 50, 5, 1.0f, 6.5f);
				DestroyObject(gameObject, detonatorObject.GetComponent<Detonator>().destroyTime); // Destroy gameobject when effect is approximately over
			} else {
				DestroyObject(gameObject);
			}
			exploded = true;
		}
	}

	public bool IsAlive()
	{
		return controller.IsAlive();
	}

	public float GetRelativeHitpoints()
	{
		return controller.GetRelativeHitpoints();
	}

	public float GetDamage()
	{
		return controller.GetDamage();
	}
}
