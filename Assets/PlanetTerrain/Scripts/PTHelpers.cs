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

	public static Vector2 GetHeightmapCoord(Vector3 p) {
		var mag = new Vector3(Mathf.Abs(p.x), Mathf.Abs(p.y), Mathf.Abs(p.z));
		if (p.x >= mag.y && p.x >= mag.z) { // Right
			var a = p/p.x;
			var x = (a.z+1f)/2f;
			var y = (a.y+1f)/2f;
			return new Vector2(Mathf.Lerp(0.501f,0.744f,x), Mathf.Lerp(0.334f,0.665f,y));
		} else if (mag.x >= mag.y && mag.x >= mag.z) { // Left
			var a = p/p.x;
			var x = (a.z+1f)/2f;
			var y = 1f-((a.y+1f)/2f);
			return new Vector2(Mathf.Lerp(0.001f,0.249f,x), Mathf.Lerp(0.334f,0.665f,y));
		} else if (p.y >= mag.x && p.y >= mag.z) { // Top
			var a = p/p.y;
			var x = (a.x+1f)/2f;
			var y = (a.z+1f)/2f;
			return new Vector2(Mathf.Lerp(0.251f,0.499f,x), Mathf.Lerp(0.666f,0.999f,y));
		} else if (mag.y >= mag.x && mag.y >= mag.z) { // Bottom
			var a = p/p.y;
			var x = 1f-((a.x+1f)/2f);
			var y = (a.z+1f)/2f;
			return new Vector2(Mathf.Lerp(0.251f,0.499f,x), Mathf.Lerp(0.001f,0.333f,y));
		} else if (p.z >= mag.x && p.z >= mag.y) { // Back
			var a = p/p.z;
			var x = 1-((a.x+1f)/2f);
			var y = (a.y+1f)/2f;
			return new Vector2(Mathf.Lerp(0.751f,0.999f,x), Mathf.Lerp(0.334f,0.665f,y));
		} else if (mag.z >= mag.x && mag.z >= mag.y) { // Front
			var a = -p/p.z;
			var x = (a.x+1f)/2f;
			var y = (a.y+1f)/2f;
			return new Vector2(Mathf.Lerp(0.251f,0.499f,x), Mathf.Lerp(0.334f,0.665f,y));
		}
		return Vector2.zero;
	}
}
