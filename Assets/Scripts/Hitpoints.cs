using UnityEngine;

public class Hitpoints : MonoBehaviour, IHitpointsUser {

    public int HitpointsValue;

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
		if (GetComponents<PlayerController>().Length == 0) {
			Debug.Log("AI Player died.");
			Destroy(gameObject);

			// TODO: Should we rewrite this to remove the PlayerController check and handle the player case by throwing an Event or something?
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
