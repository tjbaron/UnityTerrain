using UnityEngine;
using System.Collections;

public static class Segment {
	public static Mesh Generate(int segmentResolution, float radius, Vector3 xa, Vector3 xb, Vector3 ya, Vector3 yb, bool terrain=true) {
		Vector3[] newVertices = new Vector3[(segmentResolution+1)*(segmentResolution+1)];
		//Vector2[] newUV;
		int[] newTriangles = new int[segmentResolution*segmentResolution*3*2];

		
		for (int y=0; y<segmentResolution+1; y++) {
			for (int x=0; x<segmentResolution+1; x++) {
				var v = (y*(segmentResolution+1))+x;
				Vector3 p = Vector3.Lerp(
					Vector3.Lerp(xa, xb, x/(float)segmentResolution),
					Vector3.Lerp(ya, yb, x/(float)segmentResolution),
					y/(float)segmentResolution);

				if (terrain) {
					p = p.normalized;
					var noise1 = Noise.Perlin(3f*p.x,3f*p.y,3f*p.z);
					var noise2 = Noise.Perlin(11f*p.x,11f*p.y,11f*p.z);
					var noise3 = Noise.Perlin(23f*p.x,23f*p.y,23f*p.z);
					var noise4 = Noise.Perlin(71f*p.x,71f*p.y,71f*p.z);
					newVertices[v] = p.normalized * (radius + (5f*noise1) + (3f*noise2) + (2f*noise3) + (1f*noise4));
				} else newVertices[v] = p.normalized * radius;

				if (x > 0 && y > 0) {
					var i = ((x-1)*segmentResolution*6)+((y-1)*6);
					newTriangles[i] = v;
					newTriangles[i+1] = v-1;
					newTriangles[i+2] = v-segmentResolution-1;
					newTriangles[i+3] = v-1;
					newTriangles[i+4] = v-segmentResolution-2;
					newTriangles[i+5] = v-segmentResolution-1;
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		mesh.RecalculateNormals();
		return mesh;
	}
}
