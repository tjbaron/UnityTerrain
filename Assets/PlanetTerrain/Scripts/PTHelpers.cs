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
		var rad = d.planet.radius;
		var tl = d.topLeft.normalized * rad;
		var tr = d.topRight.normalized * rad;
		var bl = d.bottomLeft.normalized * rad;
		var br = d.bottomRight.normalized * rad;
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
		var rad = d.planet.radius;
		var tl = d.topLeft.normalized * rad;
		var tr = d.topRight.normalized * rad;
		var bl = d.bottomLeft.normalized * rad;
		var br = d.bottomRight.normalized * rad;
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
		var rad = d.planet.radius;
		var tl = d.topLeft.normalized * rad;
		var br = d.bottomRight.normalized * rad;
		return Vector3.Distance(tl,br);
	}
}
