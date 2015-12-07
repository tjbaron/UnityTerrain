using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Mathf.Tan(d.degreesPerQuad*Mathf.Deg2Rad)*dist < (tr-tl).magnitude/d.resolution

public class PlanetTerrainSegment : MonoBehaviour {
	public bool isEnabled = false;
	public bool subdivided = false;
	public bool generated = false;
	public bool isVisible = false;
	public SegmentData d;
	public PlanetData p;
	public MeshRenderer mr = null;
	public MeshCollider mc = null;
	public PlanetTerrainSegment[] segments = new PlanetTerrainSegment[4];

	public void Enable() {
		isEnabled = true;
		if (Application.isPlaying) {
			mc.enabled = true;
			UpdateVisibility();
		}
	}
	public void Disable() {
		if (mr.enabled) PTHelpers.segmentCount--;
		if (subdivided) {
			for (var i=0; i<segments.Length; i++) {
				segments[i].Disable();
			}
		}
		Destroy(gameObject);
	}

	public IEnumerator Generate(SegmentData newdata=null) {
		if (newdata != null) {
			d = newdata;
			p = d.planet;
		}

		if (!generated) {
			Debug.Log(d.subdivision);
			// Generate this segment.
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			if (Application.isPlaying) {
				yield return StartCoroutine(MakeSegment(d));
			} else {
				IEnumerator e = MakeSegment(d);
    			while (e.MoveNext());
			}
			generated = true;
		} else {
			yield return StartCoroutine(SubdivideDecider());
		}

		if (!Application.isPlaying && p.editorSubdivisions-d.subdivision > 0) {
			IEnumerator e = SubdivideDecider();
	    	while (e.MoveNext());
		}
	}

	void Update() {
		// Decide if this segment should be visible at all.
		if (generated && isEnabled && !subdivided) {
			UpdateVisibility();	
		}
	}

	private IEnumerator SubdivideDecider() {
		// Decide if the mesh should be subdivided
		if (!Application.isPlaying || p.minSubdivisions-d.subdivision>0 || (Application.isPlaying && isVisible && p.maxSubdivisions-d.subdivision > 0 && PTHelpers.GetDistance(d) < PTHelpers.GetSize(d))) {
			if (!subdivided) {
				if (Application.isPlaying) {
					yield return StartCoroutine(SubdivideSegment(d));
				} else {
					IEnumerator e = SubdivideSegment(d);
	    			while (e.MoveNext());
				}
			} else {
				for (var i=0; i<segments.Length; i++) {
					yield return StartCoroutine(segments[i].Generate());
				}
			}
		} else {
			if (subdivided) {
				for (var i=0; i<segments.Length; i++) {
					segments[i].Disable();
				}
				subdivided = false;
				mc.enabled = true;
				UpdateVisibility();
			}
		}
	}

	private IEnumerator SubdivideSegment(SegmentData d) {
		subdivided = true;
		// Find the 5 points that split this quad into 4 more.
		var top = (d.topLeft+d.topRight)/2f;
		var right = (d.topRight+d.bottomRight)/2f;
		var bottom = (d.bottomLeft+d.bottomRight)/2f;
		var left = (d.topLeft+d.bottomLeft)/2f;
		var mid = (d.topLeft+d.bottomRight)/2f;
		// Define the 4 new quads using existing and new points.
		var segs = new Vector3[][]{
			new Vector3[]{d.topLeft, top, left, mid},
			new Vector3[]{top, d.topRight, mid, right},
			new Vector3[]{left, mid, d.bottomLeft, bottom},
			new Vector3[]{mid, right, bottom, d.bottomRight}
		};

		for (var i=0; i<segs.Length; i++) {
			var d2 = d.subdivCopy(segs[i]);
			var go = new GameObject();
			go.GetComponent<Transform>().parent = transform;
			var seg = go.AddComponent<PlanetTerrainSegment>();
			segments[i] = seg;
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(d2));
			} else {
				IEnumerator e = seg.Generate(d2);
    			while (e.MoveNext());
			}
		}

		for (var i=0; i<segments.Length; i++) {
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
