using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Mathf.Tan(d.degreesPerQuad*Mathf.Deg2Rad)*dist < (tr-tl).magnitude/d.resolution

public class PlanetTerrainSegment : MonoBehaviour {
	public bool isEnabled = false;
	public bool isVisible = false;
	public SegmentData d;
	public PlanetData p;
	public MeshRenderer mr = null;
	public MeshCollider mc = null;
	public List<PlanetTerrainSegment> segments = new List<PlanetTerrainSegment>();

	public IEnumerator Generate(SegmentData newdata=null) {
		if (newdata != null) {
			d = newdata;
			p = d.planet;
		}

		if (mr == null) {
			//Debug.Log(d.subdivision);
			// Generate this segment.
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			if (Application.isPlaying) {
				yield return StartCoroutine(MakeSegment(d));
			} else {
				IEnumerator e = MakeSegment(d);
    			while (e.MoveNext());
			}
		} else {
			yield return StartCoroutine(SubdivideDecider());
		}

		if (!Application.isPlaying && p.editorSubdivisions-d.subdivision > 0) {
			IEnumerator e = SubdivideDecider();
	    	while (e.MoveNext());
		}
	}

	public void Enable() {
		isEnabled = true;
		if (Application.isPlaying) {
			mc.enabled = true;
			UpdateVisibility();
		}
	}

	void Update() {
		// Decide if this segment should be visible at all.
		if (isEnabled && segments.Count == 0) {
			UpdateVisibility();	
		}
	}

	public void Disable() {
		if (mr != null && mr.enabled) PTHelpers.segmentCount--;
		while (segments.Count>0) {
			segments[0].Disable();
			segments.RemoveAt(0);
		}
		DestroyImmediate(gameObject);
	}

	private IEnumerator SubdivideDecider() {
		// Decide if the mesh should be subdivided
		if (!Application.isPlaying || p.minSubdivisions-d.subdivision>0 || (Application.isPlaying && isVisible && p.maxSubdivisions-d.subdivision > 0 && PTHelpers.GetDistance(d) < PTHelpers.GetSize(d))) {
			if (segments.Count == 0) {
				if (Application.isPlaying) {
					yield return StartCoroutine(SubdivideSegment(d));
				} else {
					IEnumerator e = SubdivideSegment(d);
	    			while (e.MoveNext());
				}
			} else {
				for (var i=0; i<segments.Count; i++) {
					yield return StartCoroutine(segments[i].Generate());
				}
			}
		} else {
			if (segments.Count > 0) {
				while (segments.Count>0) {
					segments[0].Disable();
					segments.RemoveAt(0);
				}
				mc.enabled = true;
				UpdateVisibility();
			}
		}
	}

	private IEnumerator SubdivideSegment(SegmentData d) {
		// Find the 5 points that split this quad into 4 more.
		var top = (d.topLeft+d.topRight)/2f;
		var right = (d.topRight+d.bottomRight)/2f;
		var bottom = (d.bottomLeft+d.bottomRight)/2f;
		var left = (d.topLeft+d.bottomLeft)/2f;
		var mid = (d.topLeft+d.bottomRight)/2f;
		// Define the 4 new quads using existing and new points.
		var segCorners = new Vector3[][]{
			new Vector3[]{d.topLeft, top, left, mid},
			new Vector3[]{top, d.topRight, mid, right},
			new Vector3[]{left, mid, d.bottomLeft, bottom},
			new Vector3[]{mid, right, bottom, d.bottomRight}
		};

		var uCenter = new Vector2((d.uvMin.x+d.uvMax.x)/2f, (d.uvMin.y+d.uvMax.y)/2f);
		var uTop = new Vector2(uCenter.x, d.uvMin.y);
		var uLeft = new Vector2(d.uvMin.x, uCenter.y);
		var uRight = new Vector2(d.uvMax.x, uLeft.y);
		var uBottom = new Vector2(uCenter.x, d.uvMax.y);
		var uvCorners = new Vector2[][]{
			new Vector2[]{d.uvMin, uCenter},
			new Vector2[]{uTop, uRight},
			new Vector2[]{uLeft, uBottom},
			new Vector2[]{uCenter, d.uvMax}
		};

		for (var i=0; i<segCorners.Length; i++) {
			var d2 = d.subdivCopy(segCorners[i], uvCorners[i]);
			var go = new GameObject();
			go.GetComponent<Transform>().parent = transform;
			var seg = go.AddComponent<PlanetTerrainSegment>();
			segments.Add(seg);
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(d2));
			} else {
				IEnumerator e = seg.Generate(d2);
    			while (e.MoveNext());
			}
		}

		for (var i=0; i<segments.Count; i++) {
			segments[i].Enable();
		}

		if (p.minSubdivisions-d.subdivision <= 0) {
			if (mr.enabled) {
				mr.enabled = false;
				PTHelpers.segmentCount--;
			}
			mc.enabled = false;
		}
	}

	private IEnumerator MakeSegment(SegmentData d) {
		if (p.minSubdivisions-d.subdivision <= 0) {
			var mf = gameObject.AddComponent<MeshFilter>();
			mr = gameObject.AddComponent<MeshRenderer>();
			mc = gameObject.AddComponent<MeshCollider>();

			if (Application.isPlaying || p.editorSubdivisions-d.subdivision != 0) {
				mr.enabled = false;
				mr.enabled = false;
			}
			mr.receiveShadows = false;

			if (Application.isPlaying) {
				yield return StartCoroutine(SegmentGenerator.Generate(d, mf, mc));
			} else {
				IEnumerator e = SegmentGenerator.Generate(d, mf, mc);
				while (e.MoveNext());
			}
			mr.sharedMaterial = p.mainMaterial;
		}
	}

	private void UpdateVisibility() {
		var horizonAngle = Mathf.Acos(p.radius/Camera.main.transform.position.magnitude) * Mathf.Rad2Deg;
		if (!Application.isPlaying || PTHelpers.GetAngle(d) < horizonAngle+1f) {
			isVisible = true;
			if (!mr.enabled) {
				mr.enabled = true;
				PTHelpers.segmentCount++;
			}
		} else {
			isVisible = false;
			if (mr.enabled) {
				mr.enabled = false;
				PTHelpers.segmentCount--;
			}
		}
	}
}
