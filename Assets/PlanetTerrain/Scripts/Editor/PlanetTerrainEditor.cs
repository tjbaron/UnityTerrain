using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlanetTerrain))]
public class PlanetTerrainEditor : Editor {
	PlanetTerrain pt;

	enum TAB {General, Displacement, Materials, Objects};
	TAB tab = TAB.General;

	SerializedProperty r;

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

	SerializedProperty materials;

	bool customWater = false; bool customGround = false;

	void OnEnable () {
		pt = (PlanetTerrain)target;

		r = serializedObject.FindProperty("planet");
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
		serializedObject.Update();

		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		if (GUILayout.Toggle(tab == TAB.General, "General", EditorStyles.toolbarButton)) tab = TAB.General;
		if (GUILayout.Toggle(tab == TAB.Displacement, "Displacement", EditorStyles.toolbarButton)) tab = TAB.Displacement;
		if (GUILayout.Toggle(tab == TAB.Materials, "Materials", EditorStyles.toolbarButton)) tab = TAB.Materials;
		if (GUILayout.Toggle(tab == TAB.Objects, "Objects", EditorStyles.toolbarButton)) tab = TAB.Objects;
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();


		if (tab == TAB.General) GeneralTab();
		else if (tab == TAB.Displacement)  DisplacementTab();
		else if (tab == TAB.Materials) MaterialsTab();
		else if (tab == TAB.Objects) ObjectsTab();

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

	void GeneralTab() {
		EditorGUILayout.PropertyField(r.FindPropertyRelative("generateColliders"));
		EditorGUILayout.IntSlider(segmentResolution, 2, 64);
		EditorGUILayout.IntSlider(minSubdivisions, 0, maxSubdivisions.intValue);
		EditorGUILayout.IntSlider(maxSubdivisions, 0, 8);
		EditorGUILayout.IntSlider(editorSubdivisions, minSubdivisions.intValue, maxSubdivisions.intValue);
		EditorGUILayout.Slider(radius, 1f, 20000f, new GUIContent("Planet Radius"));
		EditorGUILayout.Slider(waterHeight, 0f, 500f);

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(waterMesh);
	}

	void DisplacementTab() {
		GUILayout.BeginHorizontal();
			GUILayout.Label("");
			if (GUILayout.Button("+", GUILayout.Width(30))) {
				displacements.InsertArrayElementAtIndex(displacements.arraySize);
			}
		GUILayout.EndHorizontal();

		for (var i=0; i<displacements.arraySize; i++) {
			var e = displacements.GetArrayElementAtIndex(i);
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			var b = e.isExpanded = EditorGUILayout.Foldout(e.isExpanded, "Displacement Layer "+(i+1).ToString());
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
				EditorGUILayout.PropertyField(e.FindPropertyRelative("noise"));
				EditorGUILayout.Slider(e.FindPropertyRelative("seed"), 0f, 65536f);
				EditorGUILayout.Slider(e.FindPropertyRelative("height"), 0f, radius.floatValue);
				EditorGUILayout.Slider(e.FindPropertyRelative("detail"), 0.5f, radius.floatValue, new GUIContent("Noise Frequency"));
				EditorGUILayout.PropertyField(e.FindPropertyRelative("heightStrength"));
				EditorGUILayout.PropertyField(e.FindPropertyRelative("texture"));
			}
		}
	}

	void MaterialsTab() {
		GUILayout.BeginHorizontal();
			customGround = GUILayout.Toggle(customGround, "Custom Ground");
			customWater = GUILayout.Toggle(customWater, "Custom Water");
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();
		if (customGround) {
			EditorGUILayout.PropertyField(mainMaterial);
		} else {
			EditorGUILayout.Slider(materials.FindPropertyRelative("textureTransition"), 0f, 1f);
			
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("mainTexture"), new GUIContent("Base Texture"));
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("mountainTexture"), new GUIContent("Texture 2"));
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("mountainTexture"), new GUIContent("Texture 3"));
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("poleTexture"), new GUIContent("Texture 4"));
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("poleTexture"), new GUIContent("Texture 5"));
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("poleTexture"), new GUIContent("Paint Map"));
			EditorGUILayout.PropertyField(materials.FindPropertyRelative("skyTexture"), new GUIContent("Color Variation Map"));
		}
		EditorGUILayout.Space();
		if (customWater) {
			EditorGUILayout.PropertyField(waterMaterial);
		} else {

		}
		if (GUILayout.Button("Refresh")) pt.materials.Refresh(Selection.transforms[0], pt.planet);
	}

	void ObjectsTab() {
		GUILayout.BeginHorizontal();
		GUILayout.Label("");
		if (GUILayout.Button("+", GUILayout.Width(30))) {
			
		}
		GUILayout.EndHorizontal();
	}

}
