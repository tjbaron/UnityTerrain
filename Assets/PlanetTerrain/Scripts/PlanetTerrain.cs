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
	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;
	public DisplacementLayer[] displace;
	public Material mainMaterial;
	public Material waterMaterial;
	public int resolution;
	public int maxSubdivisions;
	public SegmentData subdivCopy(Vector3[] corners) {
		var d = new SegmentData();
		d.radius = radius;
		d.topLeft = corners[0];
		d.topRight = corners[1];
		d.bottomLeft = corners[2];
		d.bottomRight = corners[3];
		d.displace = displace;
		d.mainMaterial = mainMaterial;
		d.waterMaterial = waterMaterial;
		d.resolution = resolution;
		d.maxSubdivisions = maxSubdivisions-1;
		return d;
	}
}

public class PlanetTerrain : MonoBehaviour {
	public Transform waterSphere;
	public bool updateTerrain = false;
	public int segmentResolution = 8;
	public int maxSubdivisions = 3;
	public float radius = 1000f;
	public float waterHeight = 0f;
	public Material mainMaterial;// = new Material(Shader.Find("Terrain/HeightTexture"));
	public Material waterMaterial;// = new Material(Shader.Find("Terrain/HeightTexture"));
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

	void Start() {
		UpdateTerrain();
	}

	public void UpdateTerrain() {
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

	IEnumerator MakeSphere() {
		if (waterSphere != null && waterHeight > 0f) {
			var ws = Instantiate(waterSphere);
			ws.localScale *= (radius+waterHeight)/1000f;
			ws.parent = transform;
			ws.gameObject.hideFlags = HideFlags.HideInHierarchy;
			ws.GetComponent<Renderer>().material = waterMaterial;
		}
		for (var i=0; i<6; i++) {
			var d = new SegmentData();
			d.radius = radius;
			d.topLeft = cubeSides[i][0]; d.topRight = cubeSides[i][1];
			d.bottomLeft = cubeSides[i][2]; d.bottomRight = cubeSides[i][3];
			d.displace = displacementLayers;
			d.mainMaterial = mainMaterial;
			d.resolution = segmentResolution;
			d.maxSubdivisions = maxSubdivisions;

			var go = new GameObject();
			go.GetComponent<Transform>().parent = transform;
			var seg = go.AddComponent<PlanetTerrainSegment>();
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(d));
			} else {
				IEnumerator e = seg.Generate(d);
    			while (e.MoveNext());
			}
		}
	}
}
