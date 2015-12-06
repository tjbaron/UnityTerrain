using UnityEngine;
using System.Collections;

public static class PTHelpers {
	public static int segmentCount = 6;

	public static Vector3[][] cubeSides = new Vector3[][]{
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

	public static float GetDistance(SegmentData d) {
		var tl = d.topLeft.normalized * d.radius;
		var tr = d.topRight.normalized * d.radius;
		var bl = d.bottomLeft.normalized * d.radius;
		var br = d.bottomRight.normalized * d.radius;
		var cameraPos = Camera.main.transform.position;
		return Mathf.Min(
			Vector3.Distance((tl+br)/2f, cameraPos),
			Vector3.Distance(tl, cameraPos),
			Vector3.Distance(tr, cameraPos),
			Vector3.Distance(bl, cameraPos),
			Vector3.Distance(br, cameraPos)
		);
	}

	public static float GetAngle(SegmentData d) {
		var tl = d.topLeft.normalized * d.radius;
		var tr = d.topRight.normalized * d.radius;
		var bl = d.bottomLeft.normalized * d.radius;
		var br = d.bottomRight.normalized * d.radius;
		var cameraDir = Camera.main.transform.position.normalized;	
		return Mathf.Min(
			Vector3.Angle((tl+br).normalized, cameraDir),
			Vector3.Angle(tl.normalized, cameraDir),
			Vector3.Angle(tr.normalized, cameraDir),
			Vector3.Angle(bl.normalized, cameraDir),
			Vector3.Angle(br.normalized, cameraDir)
		);
	}

	public static float GetSize(SegmentData d) {
		var tl = d.topLeft.normalized * d.radius;
		var br = d.bottomRight.normalized * d.radius;
		return Vector3.Distance(tl,br);
	}
}
