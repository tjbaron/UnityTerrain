using UnityEngine;
using System.Collections;

[System.Serializable]
public class SegmentData {
	public PlanetData planet;
	public int subdivision;
	public Vector3 topLeft;
	public Vector3 topRight;
	public Vector3 bottomLeft;
	public Vector3 bottomRight;
	public SegmentData subdivCopy(Vector3[] corners) {
		var d = new SegmentData();
		d.planet = planet;
		d.subdivision = subdivision+1;
		d.topLeft = corners[0];
		d.topRight = corners[1];
		d.bottomLeft = corners[2];
		d.bottomRight = corners[3];
		return d;
	}
}
