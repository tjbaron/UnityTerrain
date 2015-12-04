using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlanetTerrain))]
public class PlanetTerrainEditor : Editor {
	SerializedProperty segmentResolution;
	SerializedProperty maxSubdivisions;
	SerializedProperty radius;
	SerializedProperty waterHeight;

	SerializedProperty mainMaterial;
	SerializedProperty waterMaterial;
	SerializedProperty waterMesh;

	SerializedProperty displacements;
	List<bool> displacementBools = new List<bool>();

	
	void OnEnable () {
		segmentResolution = serializedObject.FindProperty("segmentResolution");
		maxSubdivisions = serializedObject.FindProperty("maxSubdivisions");
		radius = serializedObject.FindProperty("radius");
		waterHeight = serializedObject.FindProperty("waterHeight");

		mainMaterial = serializedObject.FindProperty("mainMaterial");
		waterMaterial = serializedObject.FindProperty("waterMaterial");
		waterMesh = serializedObject.FindProperty("waterSphere");

		displacements = serializedObject.FindProperty("displacementLayers");
	}

	public override void OnInspectorGUI() {
		PlanetTerrain pt = (PlanetTerrain)target;
		serializedObject.Update();

		EditorGUILayout.IntSlider(segmentResolution, 2, 64, new GUIContent("Segment Resolution"));
		EditorGUILayout.IntSlider(maxSubdivisions, 2, 6, new GUIContent("Max Subdivisions"));
		EditorGUILayout.Slider(radius, 1f, 20000f, new GUIContent("Planet Radius"));
		EditorGUILayout.Slider(waterHeight, 0f, 500f, new GUIContent("Water Height"));

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(mainMaterial);
		EditorGUILayout.PropertyField(waterMaterial);
		EditorGUILayout.PropertyField(waterMesh);

		EditorGUILayout.Space();

		for (var i=0; i<displacements.arraySize; i++) {
			if (i >= displacementBools.Count) displacementBools.Add(false);
			var b = displacementBools[i] = EditorGUILayout.Foldout(displacementBools[i], "Displacement Layer "+(i+1).ToString());
			if(b) {
				var e = displacements.GetArrayElementAtIndex(i);
				var noise = e.FindPropertyRelative("noise");
				EditorGUILayout.PropertyField(noise);
			}
		}

		EditorGUILayout.Space();
		if (GUILayout.Button("Generate")) {
			pt.UpdateTerrain();
		}
		serializedObject.ApplyModifiedProperties();
	}
}
