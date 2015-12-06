using UnityEngine;
using System.Collections;

[System.Serializable]
public class SegmentData {
	public PlanetData planet;
	public float radius;
	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;
	public DisplacementLayer[] displace;
	public Material mainMaterial;
	public Material waterMaterial;
	public int resolution;
	public int minSubdivisions;
	public int maxSubdivisions;
	public int editorSubdivisions;
	public float degreesPerQuad = 6f;
	public SegmentData subdivCopy(Vector3[] corners) {
		var d = new SegmentData();
		d.radius = radius;
		d.topLeft = corners[0];
		d.topRight = corners[1];
		d.bottomLeft = corners[2];
		d.bottomRight = corners[3];
		d.displace = displace;
		d.mainMaterial = mainMaterial;
		d.waterMaterial = waterMaterial;
		d.resolution = resolution;
		d.minSubdivisions = minSubdivisions-1;
		d.maxSubdivisions = maxSubdivisions-1;
		d.editorSubdivisions = editorSubdivisions-1;
		d.degreesPerQuad = degreesPerQuad;
		return d;
	}
}
