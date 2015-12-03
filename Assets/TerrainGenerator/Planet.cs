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

public class SegmentData {
	public float radius;
	public float waterHeight;
	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;
	public DisplacementLayer[] displace;
	public Material mainMaterial;
	public Material waterMaterial;
	public int resolution;
	public SegmentData subdivCopy(Vector3[] corners) {
		var d = new SegmentData();
		d.radius = radius;
		d.waterHeight = waterHeight;
		d.topLeft = corners[0];
		d.topRight = corners[1];
		d.bottomLeft = corners[2];
		d.bottomRight = corners[3];
		d.displace = displace;
		d.mainMaterial = mainMaterial;
		d.waterMaterial = waterMaterial;
		d.resolution = resolution;
		return d;
	}
}

[ExecuteInEditMode]
public class Planet : MonoBehaviour {
	public bool updateTerrain = false;
	public int segmentResolution = 32;
	public float radius = 32f;
	public float waterHeight = 2f;
	public Material mainMaterial;
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
			if (Application.isPlaying) {
				StartCoroutine(MakeSphere());
			} else {
				IEnumerator e = MakeSphere();
    			while (e.MoveNext());
			}
			updateTerrain = false;
		}
	}

	IEnumerator MakeSphere() {
		for (var i=0; i<6; i++) {
			var d = new SegmentData();
			d.radius = radius;
			d.waterHeight = waterHeight;
			d.topLeft = cubeSides[i][0]; d.topRight = cubeSides[i][1];
			d.bottomLeft = cubeSides[i][2]; d.bottomRight = cubeSides[i][3];
			d.displace = displacementLayers;
			d.mainMaterial = mainMaterial;
			d.waterMaterial = waterMaterial;
			d.resolution = segmentResolution;

			var go = new GameObject();
			go.GetComponent<Transform>().parent = transform;
			var seg = go.AddComponent<Segment>();
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(d));
			} else {
				IEnumerator e = seg.Generate(d);
    			while (e.MoveNext());
			}
		}
	}
}
