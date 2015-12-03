using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NOISE {Perlin,Worley}

[System.Serializable]
public class DisplacementLayer {
	public NOISE noise;
	public float seed;
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

	private Vector3[][] cubeSides = new Vector3[][]{
		new Vector3[]{
			new Vector3(-1f,-1f, 1f),
			new Vector3( 1f,-1f, 1f),
			new Vector3(-1f, 1f, 1f),
			new Vector3( 1f, 1f, 1f)
		},
		new Vector3[]{
			new Vector3(-1f, 1f, 1f), 
			new Vector3( 1f, 1f, 1f),
			new Vector3(-1f, 1f,-1f),
			new Vector3( 1f, 1f,-1f)
		},
		new Vector3[]{
			new Vector3(-1f,-1f,-1f), 
			new Vector3( 1f,-1f,-1f),
			new Vector3(-1f,-1f, 1f),
			new Vector3( 1f,-1f, 1f)
		},
		new Vector3[]{
			new Vector3( 1f,-1f,-1f), 
			new Vector3( 1f, 1f,-1f),
			new Vector3( 1f,-1f, 1f),
			new Vector3( 1f, 1f, 1f)
		},
		new Vector3[]{
			new Vector3(-1f,-1f, 1f), 
			new Vector3(-1f, 1f, 1f),
			new Vector3(-1f,-1f,-1f),
			new Vector3(-1f, 1f,-1f)
		},
		new Vector3[]{
			new Vector3( 1f,-1f,-1f), 
			new Vector3(-1f,-1f,-1f),
			new Vector3( 1f, 1f,-1f),
			new Vector3(-1f, 1f,-1f)
		}
	};

	// Use this for initialization
	void Update() {
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
			MakeSegment(1, cubeSides[i], rad, displace);
		}
	}

	void MakeSegment(int subdiv, Vector3[] v, float rad, DisplacementLayer[] displace=null) {
		// Find the 5 points that split this quad into 4 more.
		var top = (v[1]+v[0])/2f;
		var right = (v[3]+v[1])/2f;
		var bottom = (v[2]+v[3])/2f;
		var left = (v[0]+v[2])/2f;
		var mid = (v[3]+v[0])/2f;
		// Define the 4 new quads using existing and new points.
		var segs = new Vector3[][]{
			new Vector3[]{v[0], top, left, mid},
			new Vector3[]{top, v[1], mid, right},
			new Vector3[]{left, mid, v[2], bottom},
			new Vector3[]{mid, right, bottom, v[3]}
		};

		for (var i=0; i<segs.Length; i++) {
			if (subdiv == 1) {
				var seg = segs[i];
				var go = new GameObject();
				var mf = go.AddComponent<MeshFilter>();
				var mr = go.AddComponent<MeshRenderer>();
				var mc = go.AddComponent<MeshCollider>();

				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
					seg[0], 
					seg[1],
					seg[2],
					seg[3], displace);

				if (displace != null) mr.sharedMaterial = material;
				else mr.sharedMaterial = waterMaterial;
				mc.sharedMesh = mf.sharedMesh;
				go.GetComponent<Transform>().parent = transform;
			} else {
				MakeSegment(subdiv-1, segs[i], rad, displace);
			}
		}
	}
}
