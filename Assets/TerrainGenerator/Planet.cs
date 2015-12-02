using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NOISE {Perlin,Worley}

[System.Serializable]
public class DisplacementLayer {
	public NOISE noise;
	public float height;
	public float detail;
	public AnimationCurve heightStrength;
	public AnimationCurve equatorStrength;
}

[ExecuteInEditMode]
public class Planet : MonoBehaviour {
	public bool updateTerrain = false;
	public int segmentResolution = 32;
	public float radius = 32f;
	public float waterHeight = 2f;
	public Material material;
	public Material waterMaterial;
	public DisplacementLayer[] displacementLayers;

	// Use this for initialization
	void Update () {
		if (updateTerrain) {
			var children = new List<GameObject>();
			foreach (Transform child in transform) children.Add(child.gameObject);
			children.ForEach(child => DestroyImmediate(child));

			MakeSphere(radius, displacementLayers);
			if (waterHeight > 0f) MakeSphere(radius+waterHeight);
			updateTerrain = false;
		}
	}

	void MakeSphere(float rad, DisplacementLayer[] displace=null) {
		for (var i=0; i<6; i++) {
			var go = new GameObject();
			var mf = go.AddComponent<MeshFilter>();
			var mr = go.AddComponent<MeshRenderer>();
			var mc = go.AddComponent<MeshCollider>();
			if (i == 0) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,-0.5f,0.5f), 
				new Vector3(0.5f,-0.5f,0.5f),
				new Vector3(-0.5f,0.5f,0.5f),
				new Vector3(0.5f,0.5f,0.5f), displace);
			} else if (i == 1) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,0.5f,0.5f), 
				new Vector3(0.5f,0.5f,0.5f),
				new Vector3(-0.5f,0.5f,-0.5f),
				new Vector3(0.5f,0.5f,-0.5f), displace);
			} else if (i == 2) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,-0.5f,-0.5f), 
				new Vector3(0.5f,-0.5f,-0.5f),
				new Vector3(-0.5f,-0.5f,0.5f),
				new Vector3(0.5f,-0.5f,0.5f), displace);
			} else if (i == 3) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(0.5f,-0.5f,-0.5f), 
				new Vector3(0.5f,0.5f,-0.5f),
				new Vector3(0.5f,-0.5f,0.5f),
				new Vector3(0.5f,0.5f,0.5f), displace);
			} else if (i == 4) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,-0.5f,0.5f), 
				new Vector3(-0.5f,0.5f,0.5f),
				new Vector3(-0.5f,-0.5f,-0.5f),
				new Vector3(-0.5f,0.5f,-0.5f), displace);
			} else {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(0.5f,-0.5f,-0.5f), 
				new Vector3(-0.5f,-0.5f,-0.5f),
				new Vector3(0.5f,0.5f,-0.5f),
				new Vector3(-0.5f,0.5f,-0.5f), displace);
			}
			if (displace != null) mr.sharedMaterial = material;
			else mr.sharedMaterial = waterMaterial;
			mc.sharedMesh = mf.sharedMesh;
			go.GetComponent<Transform>().parent = transform;
		}
	}
}
