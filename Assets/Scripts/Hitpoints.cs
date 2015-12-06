using UnityEngine;

public class Hitpoints : MonoBehaviour {

    protected int StartHitpoints;
    public int HitpointsValue;

    void Start()
    {
        StartHitpoints = HitpointsValue; // Take over setting from editor.
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<OverrideDamage>()) {
            Decrease(collision.collider.GetComponent<OverrideDamage>().Damage);
        } else {
            Decrease((int)collision.relativeVelocity.magnitude);
        }
    }

    public void Decrease(int delta = 1)
    {
        HitpointsValue -= delta;

        if (!IsAlive() && GetComponents<PlayerController>().Length == 0)
        {
            // AI player died
            Debug.Log("AI Player died.");
            Destroy(gameObject);

            // TODO: Should we rewrite this to remove the PlayerController check and handle the player case by throwing an Event or something?
        }
    }

    public float GetRelativeHitpoints()
    {
        return (float)HitpointsValue / StartHitpoints;
    }

    public float GetDamage()
    {
        return Mathf.Clamp(1.0f - GetRelativeHitpoints(), 0.0f, 1.0f);
    }

    public bool IsAlive()
    {
        return HitpointsValue > 0;
    }

}
