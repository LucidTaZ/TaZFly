using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Hitpoints : MonoBehaviour, IHitpointsUser {

    public int HitpointsValue;
	public float DamagePerNewton = 1000;

	public GameObject DeathExplosion;

	protected bool exploded = false;
	bool isPlayer;

	protected HitpointsController controller;

    void OnEnable()
    {
		controller.SetHitpointsUser(this);
    }

	void Awake()
	{
		controller = new HitpointsController(HitpointsValue); // Take over setting from editor.
	}

	void Start()
	{
		isPlayer = GetComponent<PlayerController>();
	}

	void OnCollisionStay(Collision collision)
	{
		if (!IsAlive()) {
			// We are in free fall mode after hitpoints depletion. Anything we hit will detonate us.
			Explode();
		}
	}

    void OnCollisionEnter(Collision collision)
    {
		Debug.Log("OnCollisionEnter");
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
		Debug.Log("Starting to die...");
		Destroy(GetComponent<AutomaticSpeed>());
		Destroy(GetComponent<ShipSteeringController>());
		Destroy(GetComponent<BankByVelocity>());
		GetComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().freezeRotation = false;
		GetComponent<PropellerController>().StopSpinning();
		Debug.Log("Died.");
	}

	void Explode()
	{
		// Explode the Detonator framework explosion
		if (!exploded) {
			if (DeathExplosion) {
				Debug.Log("Going to explode...");
				GameObject detonatorObject = GameObject.Instantiate(DeathExplosion);
				DeathExplosion = null; // Make sure it only happens once!
				detonatorObject.transform.position = transform.position;
				detonatorObject.GetComponent<Detonator>().Explode();
				ShakePositional.ShakeAtLocation(transform.position, 50, 5, 1.0f, 6.5f);
				StartCoroutine(WaitThenCleanup(detonatorObject.GetComponent<Detonator>().destroyTime)); // Wait until effect is approximately over
			} else {
				DestroyObject(gameObject);
			}
			exploded = true;
		}
	}

	IEnumerator WaitThenCleanup(float seconds)
	{
		Debug.Log("Going to yield for " + seconds + " seconds");
		yield return new WaitForSeconds(seconds);
		Debug.Log("Proceding");
		DestroyObject(gameObject);
		if (isPlayer) {
			SceneManager.LoadScene("MainMenu");
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
