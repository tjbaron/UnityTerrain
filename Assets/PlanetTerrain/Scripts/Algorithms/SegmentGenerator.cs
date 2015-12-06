using UnityEngine;
using System.Collections;

public static class SegmentGenerator {
	// Lerp that isn't clamped!
	public static Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

	public static IEnumerator Generate(SegmentData d, MeshFilter mf, MeshCollider mc) {
		Vector3[] newVertices = new Vector3[(d.resolution+1)*(d.resolution+1)];
		Vector3[] newNormals = new Vector3[(d.resolution+1)*(d.resolution+1)];
		Vector2[] newUV = new Vector2[(d.resolution+1)*(d.resolution+1)];
		int[] newTriangles = new int[d.resolution*d.resolution*3*2];

		
		for (int y=0; y<d.resolution+1; y++) {
			for (int x=0; x<d.resolution+1; x++) {
				var v = (y*(d.resolution+1))+x;
				Vector3 p = Lerp(
					Lerp(d.topLeft, d.topRight, x/(float)d.resolution),
					Lerp(d.bottomLeft, d.bottomRight, x/(float)d.resolution),
					y/(float)d.resolution).normalized;

				newVertices[v] = p * GetHeight(p, d.radius, d.displace);
				newUV[v] = new Vector2(x/(float)d.resolution, y/(float)d.resolution);

				if (x > 0 && y > 0) {
					var i = ((x-1)*d.resolution*6)+((y-1)*6);
					newTriangles[i] = v;
					newTriangles[i+1] = v-1;
					newTriangles[i+2] = v-d.resolution-1;
					newTriangles[i+3] = v-1;
					newTriangles[i+4] = v-d.resolution-2;
					newTriangles[i+5] = v-d.resolution-1;
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
		for (var x=0; x<d.resolution+1; x++) {
			newNormals[x] = FindNormal(d, x, 0);
			var i = (d.resolution*(d.resolution+1))+x;
			newNormals[i] = FindNormal(d, x, d.resolution);
		}
		for (var y=0; y<d.resolution+1; y++) {
			var i = y*(d.resolution+1);
			newNormals[i] = FindNormal(d, 0, y);
			i = (y*(d.resolution+1)) + d.resolution;
			newNormals[i] = FindNormal(d, d.resolution, y);
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
					var mag = new Vector3(Mathf.Abs(p.x), Mathf.Abs(p.y), Mathf.Abs(p.z));
					if (p.x >= mag.y && p.x >= mag.z) { // Right
						var a = p/p.x;
						var x = (a.z+1f)/2f;
						var y = (a.y+1f)/2f;
						addedHeight += d.height * d.texture.GetPixelBilinear(Mathf.Lerp(0.501f,0.744f,x), Mathf.Lerp(0.334f,0.665f,y)).grayscale;
					} else if (mag.x >= mag.y && mag.x >= mag.z) { // Left
						var a = p/p.x;
						var x = (a.z+1f)/2f;
						var y = 1f-((a.y+1f)/2f);
						addedHeight += d.height * d.texture.GetPixelBilinear(Mathf.Lerp(0.001f,0.249f,x), Mathf.Lerp(0.334f,0.665f,y)).grayscale;
					} else if (p.y >= mag.x && p.y >= mag.z) { // Top
						var a = p/p.y;
						var x = (a.x+1f)/2f;
						var y = (a.z+1f)/2f;
						addedHeight += d.height * d.texture.GetPixelBilinear(Mathf.Lerp(0.251f,0.499f,x), Mathf.Lerp(0.666f,0.999f,y)).grayscale;
					} else if (mag.y >= mag.x && mag.y >= mag.z) { // Bottom
						var a = p/p.y;
						var x = 1f-((a.x+1f)/2f);
						var y = (a.z+1f)/2f;
						addedHeight += d.height * d.texture.GetPixelBilinear(Mathf.Lerp(0.251f,0.499f,x), Mathf.Lerp(0.001f,0.333f,y)).grayscale;
					} else if (p.z >= mag.x && p.z >= mag.y) { // Back
						var a = p/p.z;
						var x = 1-((a.x+1f)/2f);
						var y = (a.y+1f)/2f;
						addedHeight += d.height * d.texture.GetPixelBilinear(Mathf.Lerp(0.751f,0.999f,x), Mathf.Lerp(0.334f,0.665f,y)).grayscale;
					} else if (mag.z >= mag.x && mag.z >= mag.y) { // Front
						var a = -p/p.z;
						var x = (a.x+1f)/2f;
						var y = (a.y+1f)/2f;
						addedHeight += d.height * d.texture.GetPixelBilinear(Mathf.Lerp(0.251f,0.499f,x), Mathf.Lerp(0.334f,0.665f,y)).grayscale;
					}
				}
				maxHeight += d.height;
			}
			return radius + addedHeight;
		}
		return radius;
	}

	private static Vector3 FindNormal(SegmentData d, int x, int y) {
		var center = Lerp(
			Lerp(d.topLeft, d.topRight, x/(float)d.resolution),
			Lerp(d.bottomLeft, d.bottomRight, x/(float)d.resolution),
			y/(float)d.resolution).normalized;
		var left = Lerp(
			Lerp(d.topLeft, d.topRight, (x-1)/(float)d.resolution),
			Lerp(d.bottomLeft, d.bottomRight, (x-1)/(float)d.resolution),
			y/(float)d.resolution).normalized;
		var right = Lerp(
			Lerp(d.topLeft, d.topRight, (x+1)/(float)d.resolution),
			Lerp(d.bottomLeft, d.bottomRight, (x+1)/(float)d.resolution),
			y/(float)d.resolution).normalized;
		var top = Lerp(
			Lerp(d.topLeft, d.topRight, x/(float)d.resolution),
			Lerp(d.bottomLeft, d.bottomRight, x/(float)d.resolution),
			(y-1)/(float)d.resolution).normalized;
		var bottom = Lerp(
			Lerp(d.topLeft, d.topRight, x/(float)d.resolution),
			Lerp(d.bottomLeft, d.bottomRight, x/(float)d.resolution),
			(y+1)/(float)d.resolution).normalized;
		center = center * GetHeight(center, d.radius, d.displace);
		left = left * GetHeight(left, d.radius, d.displace);
		right = right * GetHeight(right, d.radius, d.displace);
		top = top * GetHeight(top, d.radius, d.displace);
		bottom = bottom * GetHeight(bottom, d.radius, d.displace);

		var n = Vector3.Cross(left-center, top-center);
		n += Vector3.Cross(top-center, right-center);
		n += Vector3.Cross(right-center, bottom-center);
		n += Vector3.Cross(bottom-center, left-center);
		return n.normalized;
	}
}
