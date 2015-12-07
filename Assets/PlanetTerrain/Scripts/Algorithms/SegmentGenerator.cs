using UnityEngine;
using System.Collections;

public static class SegmentGenerator {
	// Lerp that isn't clamped!
	public static Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

	public static IEnumerator Generate(SegmentData d, MeshFilter mf, MeshCollider mc) {
		var res = d.planet.segmentResolution;
		var rad = d.planet.radius;

		Vector3[] newVertices = new Vector3[(res+1)*(res+1)];
		Vector3[] newNormals = new Vector3[(res+1)*(res+1)];
		Vector2[] newUV = new Vector2[(res+1)*(res+1)];
		int[] newTriangles = new int[res*res*3*2];

		
		for (int y=0; y<res+1; y++) {
			for (int x=0; x<res+1; x++) {
				var v = (y*(res+1))+x;
				Vector3 p = Lerp(
					Lerp(d.topLeft, d.topRight, x/(float)res),
					Lerp(d.bottomLeft, d.bottomRight, x/(float)res),
					y/(float)res).normalized;

				newVertices[v] = p * GetHeight(p, rad, d.planet.displacementLayers);
				newUV[v] = new Vector2(Mathf.Lerp(d.uvMin.x, d.uvMax.x, x/(float)res), Mathf.Lerp(d.uvMin.y, d.uvMax.y, y/(float)res));

				if (x > 0 && y > 0) {
					var i = ((x-1)*res*6)+((y-1)*6);
					newTriangles[i] = v;
					newTriangles[i+1] = v-1;
					newTriangles[i+2] = v-res-1;
					newTriangles[i+3] = v-1;
					newTriangles[i+4] = v-res-2;
					newTriangles[i+5] = v-res-1;
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
		for (var x=0; x<res+1; x++) {
			newNormals[x] = FindNormal(d, x, 0);
			var i = (res*(res+1))+x;
			newNormals[i] = FindNormal(d, x, res);
		}
		for (var y=0; y<res+1; y++) {
			var i = y*(res+1);
			newNormals[i] = FindNormal(d, 0, y);
			i = (y*(res+1)) + res;
			newNormals[i] = FindNormal(d, res, y);
		}
		mesh.normals = newNormals;

		yield return null;
		mf.sharedMesh = mesh;
		mc.sharedMesh = mf.sharedMesh;
	}

	public static float GetHeight(Vector3 p, float radius, DisplacementLayer[] displace) {
		if (displace != null) {
			float maxHeight = 0f;
			float addedHeight = 0f;
			for (var i=0; i<displace.Length; i++) {
				var d = displace[i];
				var strength = 1f;
				if (maxHeight > 0f) strength = d.heightStrength.Evaluate(addedHeight/maxHeight);
				if (d.noise == NOISE.Perlin) {
					addedHeight += d.height * Noise.Perlin(d.detail*p.x+d.seed,d.detail*p.y,d.detail*p.z) * strength;
				} else if (d.noise == NOISE.Worley) {
					addedHeight += d.height * Noise.Worley(d.detail*p.x+d.seed,d.detail*p.y,d.detail*p.z) * strength;
				} else {
					var np = PTHelpers.GetHeightmapCoord(p);
					addedHeight += d.height * d.texture.GetPixelBilinear(np.x, np.y).grayscale;
				}
				maxHeight += d.height;
			}
			return radius + addedHeight;
		}
		return radius;
	}

	private static Vector3 FindNormal(SegmentData d, int x, int y) {
		var res = d.planet.segmentResolution;
		var rad = d.planet.radius;
		var displace = d.planet.displacementLayers;
		var center = Lerp(
			Lerp(d.topLeft, d.topRight, x/(float)res),
			Lerp(d.bottomLeft, d.bottomRight, x/(float)res),
			y/(float)res).normalized;
		var left = Lerp(
			Lerp(d.topLeft, d.topRight, (x-1)/(float)res),
			Lerp(d.bottomLeft, d.bottomRight, (x-1)/(float)res),
			y/(float)res).normalized;
		var right = Lerp(
			Lerp(d.topLeft, d.topRight, (x+1)/(float)res),
			Lerp(d.bottomLeft, d.bottomRight, (x+1)/(float)res),
			y/(float)res).normalized;
		var top = Lerp(
			Lerp(d.topLeft, d.topRight, x/(float)res),
			Lerp(d.bottomLeft, d.bottomRight, x/(float)res),
			(y-1)/(float)res).normalized;
		var bottom = Lerp(
			Lerp(d.topLeft, d.topRight, x/(float)res),
			Lerp(d.bottomLeft, d.bottomRight, x/(float)res),
			(y+1)/(float)res).normalized;
		center = center * GetHeight(center, rad, displace);
		left = left * GetHeight(left, rad, displace);
		right = right * GetHeight(right, rad, displace);
		top = top * GetHeight(top, rad, displace);
		bottom = bottom * GetHeight(bottom, rad, displace);

		var n = Vector3.Cross(left-center, top-center);
		n += Vector3.Cross(top-center, right-center);
		n += Vector3.Cross(right-center, bottom-center);
		n += Vector3.Cross(bottom-center, left-center);
		return n.normalized;
	}
}
