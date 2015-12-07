using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetTerrain : MonoBehaviour {
	public PlanetData planet = new PlanetData();

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
		if (planet.waterSphere != null && planet.waterHeight > 0f) {
			var ws = Instantiate(planet.waterSphere);
			ws.localScale *= (planet.radius+planet.waterHeight)/1000f;
			ws.parent = transform;
			ws.gameObject.hideFlags = HideFlags.HideInHierarchy;
			ws.GetComponent<Renderer>().material = planet.waterMaterial;
		}
		for (var i=0; i<6; i++) {
			var sd = new SegmentData();
			sd.planet = planet;
			sd.topLeft = PTHelpers.cubeSides[i][0]; sd.topRight = PTHelpers.cubeSides[i][1];
			sd.bottomLeft = PTHelpers.cubeSides[i][2]; sd.bottomRight = PTHelpers.cubeSides[i][3];

			var go = new GameObject();
			go.GetComponent<Transform>().parent = transform;
			var seg = go.AddComponent<PlanetTerrainSegment>();
			segments[i] = seg;
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(sd));
			} else {
				IEnumerator e = seg.Generate(sd);
    			while (e.MoveNext());
			}
			seg.Enable();
		}
	}

	/*void Start() {
		UpdateTerrain();
	}*/

	void Update() {
		//Debug.Log(PTHelpers.segmentCount);
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
		return SegmentGenerator.GetHeight(v, planet.radius, planet.displacementLayers);
	}
}
