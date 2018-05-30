using UnityEngine;
using System.Collections;

public class ZoomOutOnDeath : MonoBehaviour, CameraAttachmentInterface {

	public GameObject Subject;
	private bool loaded = false;
	private Hitpoints subjectHitpoints;

	private bool zoomingOut = false;

	void Awake () {
		if (Subject != null) {
			loaded = true;
			subjectHitpoints = Subject.GetComponent<Hitpoints>();
			zoomingOut = false;
		}
	}

	public void Load (GameObject subject) {
		Subject = subject;
		loaded = true;
		subjectHitpoints = Subject.GetComponent<Hitpoints>();
		zoomingOut = false;
	}

	public void Unload () {
		loaded = false;
		zoomingOut = false;
	}

	void LateUpdate () {
		if (loaded && Subject && subjectHitpoints) {
			if (!subjectHitpoints.IsAlive() && !zoomingOut) {
				zoomingOut = true;
				DisableOtherCameraScripts();
				StartCoroutine(DoZoomOut());
			}
		}
	}

	void DisableOtherCameraScripts () {
		if (GetComponent<FollowSubject>()) {
			GetComponent<FollowSubject>().enabled = false;
		}
		if (GetComponent<SwoonWithSubject>()) {
			GetComponent<SwoonWithSubject>().enabled = false;
		}
	}

	IEnumerator DoZoomOut () {
		float xDirection = transform.position.x < 0 ? 1f : -1f;
		Vector3 maxVel = new Vector3(xDirection * 5f, 6f, -20f); // Go toward center, up, back
		Vector3 maxRotVel = new Vector3(-4f, xDirection * -5f, 0f); // Look toward up, toward center, no roll

		AnimationCurve velX = Parabola(0f, 4f, maxVel.x);
		AnimationCurve velY = Parabola(0f, 4f, maxVel.y);
		AnimationCurve velZ = Parabola(0f, 4f, maxVel.z);

		AnimationCurve rotVelX = Parabola(0f, 4f, maxRotVel.x);
		AnimationCurve rotVelY = Parabola(0f, 4f, maxRotVel.x);
		AnimationCurve rotVelZ = Parabola(0f, 4f, maxRotVel.z);

		float time = 0f;

		while (true) {
			time += Time.fixedDeltaTime;
			Vector3 vel = new Vector3(
				velX.Evaluate(time),
				velY.Evaluate(time),
				velZ.Evaluate(time)
			);
			Vector3 rotVel = new Vector3(
				rotVelX.Evaluate(time),
				rotVelY.Evaluate(time),
				rotVelZ.Evaluate(time)
			);
			if (vel.sqrMagnitude < Mathf.Epsilon && rotVel.sqrMagnitude < Mathf.Epsilon) {
				break;
			}
			transform.localPosition += vel * Time.fixedDeltaTime;
			transform.localEulerAngles += rotVel * Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	private AnimationCurve Parabola (float timeStart, float timeEnd, float maxValue) {
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(new Keyframe(timeStart, 0f));
		curve.AddKey(new Keyframe(timeStart + 1 * (timeEnd - timeStart) / 4, 3 * maxValue / 4));
		curve.AddKey(new Keyframe(timeStart + 2 * (timeEnd - timeStart) / 4,     maxValue    ));
		curve.AddKey(new Keyframe(timeStart + 3 * (timeEnd - timeStart) / 4, 3 * maxValue / 4));
		curve.AddKey(new Keyframe(timeEnd, 0f));
		return curve;
	}
}
