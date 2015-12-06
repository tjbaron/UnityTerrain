using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetTerrainSegment : MonoBehaviour {
	public bool subdivided = false;
	public bool first = true;
	public SegmentData d;
	public MeshRenderer mr = null;

	public PlanetTerrainSegment[] segments = new PlanetTerrainSegment[4];

	public IEnumerator Generate(SegmentData newdata=null) {
		if (first) gameObject.hideFlags = HideFlags.HideInHierarchy;
		if (newdata != null) d = newdata;
		var tl = d.topLeft.normalized * d.radius;
		var tr = d.topRight.normalized * d.radius;
		var bl = d.bottomLeft.normalized * d.radius;
		var br = d.bottomRight.normalized * d.radius;

		var cameraPos = Camera.main.transform.position;
		var dist = Mathf.Min(
			Vector3.Distance((tl+br)/2f, cameraPos),
			Vector3.Distance(tl, cameraPos),
			Vector3.Distance(tr, cameraPos),
			Vector3.Distance(bl, cameraPos),
			Vector3.Distance(br, cameraPos)
		);

		//Debug.Log(dist);
		//Debug.Log(Mathf.Tan(d.degreesPerQuad*Mathf.Deg2Rad)*dist);
		if (
			(!Application.isPlaying && d.editorSubdivisions > 0) || 
			(
				!first && (d.maxSubdivisions > 0) && 
				//Mathf.Tan(d.degreesPerQuad*Mathf.Deg2Rad)*dist < (tr-tl).magnitude/d.resolution
				dist < Vector3.Distance(tl, br)
			)
		) {
			if (!subdivided) {
				// Subdivide the segment if it's close to the camera.
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
			// Draw the segment if it doesn't need more subdividing
			// and is facing the camera and is within our view distance.
			if (first || subdivided == true) {
				if (Application.isPlaying) {
					yield return StartCoroutine(MakeSegment(d));
				} else {
					IEnumerator e = MakeSegment(d);
	    			while (e.MoveNext());
				}
			}
		}

		// Decide if this segment should be visible at all.
		if (mr != null) {
			var cameraDir = Camera.main.transform.position.normalized;			
			var horizonAngle = Mathf.Acos(d.radius/Camera.main.transform.position.magnitude) * Mathf.Rad2Deg;

			var a1 = Mathf.Min(
				Vector3.Angle((tl+br).normalized, cameraDir),
				Vector3.Angle(tl.normalized, cameraDir),
				Vector3.Angle(tr.normalized, cameraDir),
				Vector3.Angle(bl.normalized, cameraDir),
				Vector3.Angle(br.normalized, cameraDir)
			);

			if (!Application.isPlaying || a1 < horizonAngle+1f) {
				if (mr.enabled == false) PTHelpers.Add();
				mr.enabled = true;
			} else {
				if (mr.enabled == true) PTHelpers.Remove();
				mr.enabled = false;
			}
		}

		first = false;
	}

	IEnumerator SubdivideSegment(SegmentData d) {
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);

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

		children.ForEach(child => DestroyImmediate(child));
		DestroyImmediate(gameObject.GetComponent<MeshCollider>());
		DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
		DestroyImmediate(gameObject.GetComponent<MeshFilter>());
	}

	public IEnumerator MakeSegment(SegmentData d) {
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		subdivided = false;

		var mf = gameObject.AddComponent<MeshFilter>();
		mr = gameObject.AddComponent<MeshRenderer>();
		mr.receiveShadows = false;
		var mc = gameObject.AddComponent<MeshCollider>();

		if (Application.isPlaying) {
			yield return StartCoroutine(SegmentGenerator.Generate(d.resolution,
				d.radius, 
				d.topLeft, 
				d.topRight,
				d.bottomLeft,
				d.bottomRight,
				d.displace,
				mf, mc));
		} else {
			IEnumerator e = SegmentGenerator.Generate(d.resolution,
				d.radius, 
				d.topLeft, 
				d.topRight,
				d.bottomLeft,
				d.bottomRight,
				d.displace,
				mf, mc);
			while (e.MoveNext());
		}
		mr.sharedMaterial = d.mainMaterial;
		PTHelpers.Add();

		children.ForEach(child => {
			if (child.GetComponent<MeshRenderer>().enabled) PTHelpers.Remove();
			DestroyImmediate(child);
		});
	}
}
