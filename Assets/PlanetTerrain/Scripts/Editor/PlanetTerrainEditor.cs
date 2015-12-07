using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlanetTerrain))]
public class PlanetTerrainEditor : Editor {
	enum TAB {General, Displacement, Objects};
	TAB tab = TAB.General;

	SerializedProperty segmentResolution;
	SerializedProperty minSubdivisions;
	SerializedProperty maxSubdivisions;
	SerializedProperty editorSubdivisions;
	SerializedProperty radius;
	SerializedProperty waterHeight;

	SerializedProperty mainMaterial;
	SerializedProperty waterMaterial;
	SerializedProperty waterMesh;

	SerializedProperty displacements;
	List<bool> displacementBools = new List<bool>();

	
	void OnEnable () {
		var r = serializedObject.FindProperty("planet");
		segmentResolution = r.FindPropertyRelative("segmentResolution");
		minSubdivisions = r.FindPropertyRelative("minSubdivisions");
		maxSubdivisions = r.FindPropertyRelative("maxSubdivisions");
		editorSubdivisions = r.FindPropertyRelative("editorSubdivisions");
		radius = r.FindPropertyRelative("radius");
		waterHeight = r.FindPropertyRelative("waterHeight");

		mainMaterial = r.FindPropertyRelative("mainMaterial");
		waterMaterial = r.FindPropertyRelative("waterMaterial");
		waterMesh = r.FindPropertyRelative("waterSphere");

		displacements = r.FindPropertyRelative("displacementLayers");
	}

	public override void OnInspectorGUI() {
		PlanetTerrain pt = (PlanetTerrain)target;
		serializedObject.Update();

		GUILayout.BeginHorizontal();
		if (tab == TAB.General) GUI.color = Color.green;
		if (GUILayout.Button("General")) tab = TAB.General;
		GUI.color = Color.white;

		if (tab == TAB.Displacement) GUI.color = Color.green;
		if (GUILayout.Button("Displacement")) tab = TAB.Displacement;
		GUI.color = Color.white;

		if (tab == TAB.Objects) GUI.color = Color.green;
		if (GUILayout.Button("Objects")) tab = TAB.Objects;
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		if (tab == TAB.General) {
			EditorGUILayout.IntSlider(segmentResolution, 2, 64);
			EditorGUILayout.IntSlider(minSubdivisions, 0, maxSubdivisions.intValue);
			EditorGUILayout.IntSlider(maxSubdivisions, 0, 8);
			EditorGUILayout.IntSlider(editorSubdivisions, minSubdivisions.intValue, maxSubdivisions.intValue);
			EditorGUILayout.Slider(radius, 1f, 20000f, new GUIContent("Planet Radius"));
			EditorGUILayout.Slider(waterHeight, 0f, 500f);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(mainMaterial);
			EditorGUILayout.PropertyField(waterMaterial);
			EditorGUILayout.PropertyField(waterMesh);
		} else if (tab == TAB.Displacement) {
			for (var i=0; i<displacements.arraySize; i++) {
				if (i >= displacementBools.Count) displacementBools.Add(false);
				var b = displacementBools[i] = EditorGUILayout.Foldout(displacementBools[i], "Displacement Layer "+(i+1).ToString());
				if(b) {
					GUILayout.BeginHorizontal();
						if (i > 0 && GUILayout.Button("^", GUILayout.Width(30))) {
							displacements.MoveArrayElement(i, i-1);
							break;
						}
						if (i+1 < displacements.arraySize && GUILayout.Button("v", GUILayout.Width(30))) {
							displacements.MoveArrayElement(i, i+1);
							break;
						}
						if (GUILayout.Button("-", GUILayout.Width(30))) {
							displacements.DeleteArrayElementAtIndex(i);
							break;
						}
					GUILayout.EndHorizontal();

					var e = displacements.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(e.FindPropertyRelative("noise"));
					EditorGUILayout.Slider(e.FindPropertyRelative("seed"), 0f, 65536f);
					EditorGUILayout.Slider(e.FindPropertyRelative("height"), 0f, radius.floatValue);
					EditorGUILayout.Slider(e.FindPropertyRelative("detail"), 0.5f, radius.floatValue, new GUIContent("Noise Frequency"));
					EditorGUILayout.PropertyField(e.FindPropertyRelative("heightStrength"));
					EditorGUILayout.PropertyField(e.FindPropertyRelative("texture"));
				}
			}
			EditorGUILayout.Space();
			if (GUILayout.Button("New Displacement")) {
				displacements.InsertArrayElementAtIndex(displacements.arraySize);
			}
		}

		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Generate")) {
				pt.UpdateTerrain();
			}
			if (GUILayout.Button("Clear")) {
				pt.ClearTerrain();
			}
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}
