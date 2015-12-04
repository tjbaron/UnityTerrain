using UnityEngine;
using System.Collections;

public class Segment : MonoBehaviour {
	public IEnumerator Generate(SegmentData d) {
		var tl = d.topLeft.normalized * d.radius;
		var tr = d.topRight.normalized * d.radius;
		var bl = d.bottomLeft.normalized * d.radius;
		var br = d.bottomRight.normalized * d.radius;

		var segmentCenter = (tl+br)/2f;
		var cameraPos = Camera.main.transform.position;
		
		var dist = Vector3.Distance(segmentCenter, cameraPos);
		var dist1 = Vector3.Distance(tl, cameraPos);
		var dist2 = Vector3.Distance(tr, cameraPos);
		var dist3 = Vector3.Distance(bl, cameraPos);
		var dist4 = Vector3.Distance(br, cameraPos);
		
		var size = Vector3.Distance(tl, br);

		if (size > 0.5f && (dist < size || dist1 < size || dist2 < size || dist3 < size || dist4 < size)) {
			// Subdivide the segment if it's close to the camera.
			if (Application.isPlaying) {
				yield return StartCoroutine(SubdivideSegment(d));
			} else {
				IEnumerator e = SubdivideSegment(d);
    			while (e.MoveNext());
			}
		} else /*if (dist < 15f || dist1 < 15f || dist2 < 15f || dist3 < 15f || dist4 < 15f)*/ {
			// Draw the segment if it doesn't need more subdividing
			// and is facing the camera and is within our view distance.
			if (Application.isPlaying) {
				yield return StartCoroutine(MakeSegment(d));
			} else {
				IEnumerator e = MakeSegment(d);
    			while (e.MoveNext());
			}
			if (d.waterHeight > 0f) {
				var go = new GameObject();
				go.GetComponent<Transform>().parent = transform;
				var seg = go.AddComponent<Segment>();
				if (Application.isPlaying) {
					yield return StartCoroutine(seg.MakeSegment(d, true));
				} else {
					IEnumerator e = seg.MakeSegment(d, true);
	    			while (e.MoveNext());
				}
			}
		}
	}

	IEnumerator SubdivideSegment(SegmentData d) {
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
			var seg = go.AddComponent<Segment>();
			if (Application.isPlaying) {
				yield return StartCoroutine(seg.Generate(d2));
			} else {
				IEnumerator e = seg.Generate(d2);
    			while (e.MoveNext());
			}
		}
	}

	public IEnumerator MakeSegment(SegmentData d, bool isWater=false) {
		var tl = d.topLeft.normalized * d.radius;
		var br = d.bottomRight.normalized * d.radius;
		var segmentNormal = (tl+br).normalized;
		var cameraDir = Camera.main.transform.forward;

		if (Vector3.Angle(segmentNormal, cameraDir) > 90f) {
			var mf = gameObject.AddComponent<MeshFilter>();
			var mr = gameObject.AddComponent<MeshRenderer>();
			var mc = gameObject.AddComponent<MeshCollider>();

			if (Application.isPlaying) {
				yield return StartCoroutine(SegmentGenerator.Generate(d.resolution,
					isWater?d.radius+d.waterHeight:d.radius, 
					d.topLeft, 
					d.topRight,
					d.bottomLeft,
					d.bottomRight,
					isWater?null:d.displace,
					mf, mc));
			} else {
				IEnumerator e = SegmentGenerator.Generate(d.resolution,
					isWater?d.radius+d.waterHeight:d.radius, 
					d.topLeft, 
					d.topRight,
					d.bottomLeft,
					d.bottomRight,
					isWater?null:d.displace,
					mf, mc);
				while (e.MoveNext());
			}

			if (isWater) mr.sharedMaterial = d.waterMaterial;
			else mr.sharedMaterial = d.mainMaterial;
		}
	}
}
