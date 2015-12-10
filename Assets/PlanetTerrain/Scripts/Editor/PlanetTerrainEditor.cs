using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlanetTerrain))]
public class PlanetTerrainEditor : Editor {
	enum TAB {General, Displacement, Materials, Objects};
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

	SerializedProperty materials;

	bool customWater = false; bool customGround = false;

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
		materials = serializedObject.FindProperty("materials");
	}

	public override void OnInspectorGUI() {
		PlanetTerrain pt = (PlanetTerrain)target;
		serializedObject.Update();

		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		if (GUILayout.Toggle(tab == TAB.General, "General", EditorStyles.toolbarButton)) tab = TAB.General;
		if (GUILayout.Toggle(tab == TAB.Displacement, "Displacement", EditorStyles.toolbarButton)) tab = TAB.Displacement;
		if (GUILayout.Toggle(tab == TAB.Materials, "Materials", EditorStyles.toolbarButton)) tab = TAB.Materials;
		if (GUILayout.Toggle(tab == TAB.Objects, "Objects", EditorStyles.toolbarButton)) tab = TAB.Objects;
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
			EditorGUILayout.PropertyField(waterMesh);
		} else if (tab == TAB.Displacement) {
			GUILayout.BeginHorizontal();
				GUILayout.Label("");
				if (GUILayout.Button("+", GUILayout.Width(30))) {
					displacements.InsertArrayElementAtIndex(displacements.arraySize);
				}
			GUILayout.EndHorizontal();

			for (var i=0; i<displacements.arraySize; i++) {
				EditorGUILayout.Space();
				if (i >= displacementBools.Count) displacementBools.Add(false);
				GUILayout.BeginHorizontal();
				var b = displacementBools[i] = EditorGUILayout.Foldout(displacementBools[i], "Displacement Layer "+(i+1).ToString());
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
				if(b) {
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
		} else if (tab == TAB.Materials) {
			GUILayout.BeginHorizontal();
				customGround = GUILayout.Toggle(customGround, "Custom Ground");
				customWater = GUILayout.Toggle(customWater, "Custom Water");
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			if (customGround) {
				EditorGUILayout.PropertyField(mainMaterial);
			} else {
				EditorGUILayout.Slider(materials.FindPropertyRelative("textureTransition"), 0f, 1f);
				EditorGUILayout.PropertyField(materials.FindPropertyRelative("mainTexture"));
			}
			EditorGUILayout.Space();
			if (customWater) {
				EditorGUILayout.PropertyField(waterMaterial);
			} else {

			}
		} else if (tab == TAB.Objects) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("");
			if (GUILayout.Button("+", GUILayout.Width(30))) {
				
			}
			GUILayout.EndHorizontal();
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
