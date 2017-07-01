using System;
using UnityEngine;

public class HitpointsController
{
	protected int StartHitpoints;
	public int HitpointsValue;

	protected IHitpointsUser hitpointsUser;

	public HitpointsController(int hp)
	{
		StartHitpoints = hp;
		HitpointsValue = hp;
	}

	public void SetHitpointsUser(IHitpointsUser hu)
	{
		hitpointsUser = hu;
	}

	public void Decrease(int delta = 1)
	{
		HitpointsValue -= delta;

		if (!IsAlive())
		{
			hitpointsUser.Die();
		}
	}

	public bool IsAlive()
	{
		return HitpointsValue > 0;
	}

	public float GetRelativeHitpoints()
	{
		return (float)HitpointsValue / StartHitpoints;
	}

	public float GetDamage()
	{
		return Mathf.Clamp(1.0f -GetRelativeHitpoints(), 0.0f, 1.0f);
	}
}
