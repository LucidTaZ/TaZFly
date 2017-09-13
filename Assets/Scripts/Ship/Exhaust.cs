using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Exhaust : MonoBehaviour {
	TrailRenderer trailRenderer;
	float originalTrailTime;

	void Awake () {
		trailRenderer = GetComponent<TrailRenderer>();
		originalTrailTime = trailRenderer.time;
		trailRenderer.time = 0.0f;
	}

	// Severity from 0 to 1
	public void SetSeverity (float severity) {
		trailRenderer.widthMultiplier = severity;
		trailRenderer.time = severity * originalTrailTime;
	}
}
