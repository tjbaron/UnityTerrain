using UnityEngine;
using System.Collections;

public static class Noise {
	private static float interp(float v) {
		float t = 1f-Mathf.Abs(v);
		return 3*Mathf.Pow(t,2f) - 2*Mathf.Pow(t,3f);
	}

	public static float Perlin(float x, float y, float z, bool fast=false) {
		var x0 = (int)Mathf.Floor(x);
		var y0 = (int)Mathf.Floor(y);
		var z0 = (int)Mathf.Floor(z);
		// Get vectors for 8 surrounding points.
		var total = 0f;
		for (var i=0; i<2; i++) {
			for (var j=0; j<2; j++) {
				for (var k=0; k<2; k++) {
					Random.seed = (int)((x0+i) + ((y0+j)*1000) + ((z0+k)*1000000));
					var dist = new Vector3(x-((float)x0+i), y-((float)y0+j), z-((float)z0+k));
					var val = Vector3.Dot(dist, Random.insideUnitSphere.normalized);
					if (fast) total += val*(1f-Mathf.Abs(dist.x))*(1f-Mathf.Abs(dist.y))*(1f-Mathf.Abs(dist.z));
					else total += val*interp(dist.x)*interp(dist.y)*interp(dist.z);
				}
			}
		}
		return (total+1f)/2f;
	}
}
