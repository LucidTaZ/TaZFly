﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {
	void OnSceneGUI() {
		LevelGenerator generator = (LevelGenerator) target;
		Handles.DrawPolyLine(
			generator.transform.position + new Vector3(-generator.Width, 0, 0),
			generator.transform.position + new Vector3(-generator.Width, 0, generator.Length),
			generator.transform.position + new Vector3(0, 0, generator.Length),
			generator.transform.position + new Vector3(0, 0, 0),
			generator.transform.position + new Vector3(-generator.Width, 0, 0)
		);

		Handles.DrawPolyLine(
			generator.transform.position + new Vector3(0, 0, 0),
			generator.transform.position + new Vector3(0, 0, generator.Length),
			generator.transform.position + new Vector3(generator.Width, 0, generator.Length),
			generator.transform.position + new Vector3(generator.Width, 0, 0),
			generator.transform.position + new Vector3(0, 0, 0)
		);
	}
}
