using UnityEngine;

public class Hitpoints : MonoBehaviour, IHitpointsUser {

    public int HitpointsValue;

	public GameObject DeathExplosion;

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
        if (collision.collider.GetComponent<OverrideDamage>()) {
            controller.Decrease(collision.collider.GetComponent<OverrideDamage>().Damage);
        } else {
			controller.Decrease((int)collision.relativeVelocity.magnitude);
        }
    }

	public void Die ()
	{
		// Explode the Detonator framework explosion
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
