using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetTerrain : MonoBehaviour {
	public Transform waterSphere;
	public int segmentResolution = 8;
	public int minSubdivisions = 1;
	public int maxSubdivisions = 3;
	public int editorSubdivisions = 1;
	public float degreesPerQuad = 6f;
	public float radius = 1000f;
	public float waterHeight = 0f;
	public Material mainMaterial;
	public Material waterMaterial;
	public DisplacementLayer[] displacementLayers;

	private bool busy = false;
	public PlanetTerrainSegment[] segments = new PlanetTerrainSegment[6];

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
	}

	private IEnumerator MakeSphere() {
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
			d.topLeft = PTHelpers.cubeSides[i][0]; d.topRight = PTHelpers.cubeSides[i][1];
			d.bottomLeft = PTHelpers.cubeSides[i][2]; d.bottomRight = PTHelpers.cubeSides[i][3];
			d.displace = displacementLayers;
			d.mainMaterial = mainMaterial;
			d.resolution = segmentResolution;
			d.minSubdivisions = minSubdivisions;
			d.maxSubdivisions = maxSubdivisions;
			d.editorSubdivisions = editorSubdivisions;

			var go = new GameObject();
			go.GetComponent<Transform>().parent = transform;
			var seg = go.AddComponent<PlanetTerrainSegment>();
			segments[i] = seg;
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(d));
			} else {
				IEnumerator e = seg.Generate(d);
    			while (e.MoveNext());
			}
			seg.Enable();
		}
	}

	void Update() {
		Debug.Log(PTHelpers.segmentCount);
		if (!busy) {
			StartCoroutine(RefreshTerrain());
		}
	}

	private IEnumerator RefreshTerrain() {
		busy = true;
		for (var i=0; i<segments.Length; i++) {
			yield return StartCoroutine(segments[i].Generate());
		}
		busy = false;
	}

	public float GetHeight(Vector3 v) {
		return SegmentGenerator.GetHeight(v, radius, displacementLayers);
	}
}
