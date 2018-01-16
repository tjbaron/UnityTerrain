using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PlanetTerrain : MonoBehaviour {
	public PlanetData planet = new PlanetData();
	public bool simpleView = true;
	public MaterialHandler materials = new MaterialHandler();

	private bool busy = false;
	public List<PlanetTerrainSegment> segments = new List<PlanetTerrainSegment>();
	public Transform water = null;

	public void UpdateTerrain() {
		while (segments.Count>0) {
			segments[0].Disable();
			segments.RemoveAt(0);
		}
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

	public void ClearTerrain() {
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		children.ForEach(child => DestroyImmediate(child));
		segments = new List<PlanetTerrainSegment>();
	}

	private IEnumerator MakeSphere() {
		busy = true;
		if (planet.waterSphere != null && planet.waterHeight > 0f) {
			water = Instantiate(planet.waterSphere);
			water.parent = transform; water.localPosition = Vector3.zero;
			water.localRotation = Quaternion.identity; water.localScale = new Vector3(1f,1f,1f)*(planet.radius+planet.waterHeight)/1000f;
			if (simpleView) water.gameObject.hideFlags = HideFlags.HideInHierarchy;
			water.GetComponent<Renderer>().material = planet.waterMaterial;
		}
		for (var i=0; i<6; i++) {
			var sd = new SegmentData();
			sd.planet = planet;
			sd.topLeft = PTHelpers.cubeSides[i][0]; sd.topRight = PTHelpers.cubeSides[i][1];
			sd.bottomLeft = PTHelpers.cubeSides[i][2]; sd.bottomRight = PTHelpers.cubeSides[i][3];
			sd.uvMin = Vector2.zero; sd.uvMax = new Vector2(1f,1f);

			if (!Application.isPlaying) {
				var t = SegmentGenerator.GenerateTexture(sd);
				byte[] bytes = t.EncodeToPNG();
				File.WriteAllBytes(Application.dataPath + "/../Planet"+i.ToString()+".png", bytes);
				t.Apply();
				sd.texture = t;
				materials.heightTexture = t;
			}

			var go = new GameObject(); var tr = go.GetComponent<Transform>();
			tr.parent = transform; tr.localPosition = Vector3.zero;
			tr.localRotation = Quaternion.identity; tr.localScale = new Vector3(1f,1f,1f);

			var seg = go.AddComponent<PlanetTerrainSegment>();
			segments.Add(seg);
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(sd));
			} else {
				IEnumerator e = seg.Generate(sd);
    			while (e.MoveNext());
			}
			seg.Enable();
		}
		busy = false;
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
		for (var i=0; i<segments.Count; i++) {
			yield return StartCoroutine(segments[i].Generate());
		}
		busy = false;
	}

	public float GetHeight(Vector3 v) {
		return SegmentGenerator.GetHeight(v, planet.radius, planet.displacementLayers);
	}
}
