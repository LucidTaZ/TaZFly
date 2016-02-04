using UnityEngine;

public class PropellerController : MonoBehaviour {

	public Animator PropellerAnimator;

	bool stopping = false;

	void FixedUpdate () {
		if (stopping) {
			PropellerAnimator.SetFloat("Speed", Mathf.Lerp(0, PropellerAnimator.GetFloat("Speed"), 0.5f * Time.deltaTime));
		} else {
			if (GetComponent<AutomaticSpeed>()) {
				PropellerAnimator.SetFloat("Speed", GetComponent<AutomaticSpeed>().GetRelativeSpeed() + 0.1f); // This Animator wants a value from 0 to 1.
			} else {
				// When a player dies, the AutomaticSpeed script gets detached. Therefore this situation can occur.
				PropellerAnimator.SetFloat("Speed", 1);
			}
		}
	}

	public void StopSpinning()
	{
		stopping = true;
	}
}
