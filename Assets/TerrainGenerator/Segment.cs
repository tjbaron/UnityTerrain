using UnityEngine;
using System.Collections;

public static class Segment {
	public static Mesh Generate(int segmentResolution, float radius, Vector3 xa, Vector3 xb, Vector3 ya, Vector3 yb, DisplacementLayer[] displace=null) {
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

				if (displace != null) {
					float maxHeight = 0f;
					float addedHeight = 0f;
					for (var i=0; i<displace.Length; i++) {
						var d = displace[i];
						var strength = 1f;
						if (maxHeight > 0f) strength = d.heightStrength.Evaluate(addedHeight/maxHeight);
						addedHeight += d.height * Noise.Perlin(d.detail*p.x,d.detail*p.y,d.detail*p.z) * strength;
						maxHeight += d.height;
					}
					newVertices[v] = p.normalized * (radius + addedHeight);
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
