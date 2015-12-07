using UnityEngine;
using System.Collections;
using System.IO;

public static class NormalGenerator {
	// Lerp that isn't clamped!
	public static Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

	public static IEnumerator Generate(SegmentData d, MeshFilter mf, MeshCollider mc) {
		/*var res = d.planet.segmentResolution;
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
				newUV[v] = new Vector2(x/(float)res, y/(float)res);

				if (x > 0 && y > 0) {
					var i = ((x-1)*res*6)+((y-1)*6);
					newTriangles[i] = v;
					newTriangles[i+1] = v-1;
					newTriangles[i+2] = v-res-1;
					newTriangles[i+3] = v-1;
					newTriangles[i+4] = v-res-2;
					newTriangles[i+5] = v-res-1;
				}
			}
		}

		byte[] bytes = tex.EncodeToPNG();		
		File.WriteAllBytes(Application.dataPath + "/PlanetTex.png", bytes);*/
		return null;
	}
}
