using UnityEngine;
using System.Collections;

public static class SegmentGenerator {
	// Lerp that isn't clamped!
	public static Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

	public static IEnumerator Generate(int segmentResolution, float radius, Vector3 xa, Vector3 xb, Vector3 ya, Vector3 yb, DisplacementLayer[] displace, MeshFilter mf, MeshCollider mc) {
		Vector3[] newVertices = new Vector3[(segmentResolution+1)*(segmentResolution+1)];
		Vector3[] newNormals = new Vector3[(segmentResolution+1)*(segmentResolution+1)];
		Vector2[] newUV = new Vector2[(segmentResolution+1)*(segmentResolution+1)];
		int[] newTriangles = new int[segmentResolution*segmentResolution*3*2];

		
		for (int y=0; y<segmentResolution+1; y++) {
			for (int x=0; x<segmentResolution+1; x++) {
				var v = (y*(segmentResolution+1))+x;
				Vector3 p = Lerp(
					Lerp(xa, xb, x/(float)segmentResolution),
					Lerp(ya, yb, x/(float)segmentResolution),
					y/(float)segmentResolution).normalized;

				newVertices[v] = p * GetHeight(p, radius, displace);
				newUV[v] = new Vector2(x/(float)segmentResolution, y/(float)segmentResolution);

				if (x > 0 && y > 0) {
					var i = ((x-1)*segmentResolution*6)+((y-1)*6);
					newTriangles[i] = v;
					newTriangles[i+1] = v-1;
					newTriangles[i+2] = v-segmentResolution-1;
					newTriangles[i+3] = v-1;
					newTriangles[i+4] = v-segmentResolution-2;
					newTriangles[i+5] = v-segmentResolution-1;
				}
				//if (y%1024==0) yield return null;
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		mesh.uv = newUV;
		mesh.RecalculateNormals();

		newNormals = mesh.normals;
		for (var x=0; x<segmentResolution+1; x++) {
			newNormals[x] = FindNormal(segmentResolution, radius, x, 0, xa, xb, ya, yb, displace);
			var i = (segmentResolution*(segmentResolution+1))+x;
			newNormals[i] = FindNormal(segmentResolution, radius, x, segmentResolution, xa, xb, ya, yb, displace);
		}
		for (var y=0; y<segmentResolution+1; y++) {
			var i = y*(segmentResolution+1);
			newNormals[i] = FindNormal(segmentResolution, radius, 0, y, xa, xb, ya, yb, displace);
			i = (y*(segmentResolution+1)) + segmentResolution;
			newNormals[i] = FindNormal(segmentResolution, radius, segmentResolution, y, xa, xb, ya, yb, displace);
		}
		mesh.normals = newNormals;

		yield return null;
		mf.sharedMesh = mesh;
		mc.sharedMesh = mf.sharedMesh;
	}

	private static float GetHeight(Vector3 p, float radius, DisplacementLayer[] displace) {
		if (displace != null) {
			float maxHeight = 0f;
			float addedHeight = 0f;
			for (var i=0; i<displace.Length; i++) {
				var d = displace[i];
				var strength = 1f;
				if (maxHeight > 0f) strength = d.heightStrength.Evaluate(addedHeight/maxHeight);
				if (d.noise == NOISE.Perlin) {
					addedHeight += d.height * Noise.Perlin(d.detail*p.x+d.seed,d.detail*p.y,d.detail*p.z) * strength;
				} else {
					addedHeight += d.height * Noise.Worley(d.detail*p.x+d.seed,d.detail*p.y,d.detail*p.z) * strength;
				}
				maxHeight += d.height;
			}
			return radius + addedHeight;
		}
		return radius;
	}

	private static Vector3 FindNormal(int segmentResolution, float radius, int x, int y, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, DisplacementLayer[] displace) {
		var center = Lerp(
			Lerp(topLeft, topRight, x/(float)segmentResolution),
			Lerp(bottomLeft, bottomRight, x/(float)segmentResolution),
			y/(float)segmentResolution).normalized;
		var left = Lerp(
			Lerp(topLeft, topRight, (x-1)/(float)segmentResolution),
			Lerp(bottomLeft, bottomRight, (x-1)/(float)segmentResolution),
			y/(float)segmentResolution).normalized;
		var right = Lerp(
			Lerp(topLeft, topRight, (x+1)/(float)segmentResolution),
			Lerp(bottomLeft, bottomRight, (x+1)/(float)segmentResolution),
			y/(float)segmentResolution).normalized;
		var top = Lerp(
			Lerp(topLeft, topRight, x/(float)segmentResolution),
			Lerp(bottomLeft, bottomRight, x/(float)segmentResolution),
			(y-1)/(float)segmentResolution).normalized;
		var bottom = Lerp(
			Lerp(topLeft, topRight, x/(float)segmentResolution),
			Lerp(bottomLeft, bottomRight, x/(float)segmentResolution),
			(y+1)/(float)segmentResolution).normalized;
		center = center * GetHeight(center, radius, displace);
		left = left * GetHeight(left, radius, displace);
		right = right * GetHeight(right, radius, displace);
		top = top * GetHeight(top, radius, displace);
		bottom = bottom * GetHeight(bottom, radius, displace);

		var n = Vector3.Cross(left-center, top-center);
		n += Vector3.Cross(top-center, right-center);
		n += Vector3.Cross(right-center, bottom-center);
		n += Vector3.Cross(bottom-center, left-center);
		return n.normalized;
	}
}
