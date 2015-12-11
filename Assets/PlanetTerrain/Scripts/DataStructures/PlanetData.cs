using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlanetData {
	public bool generateColliders = true;
	public Transform waterSphere;
	public int segmentResolution = 8;
	public int minSubdivisions = 1;
	public int maxSubdivisions = 3;
	public int editorSubdivisions = 1;
	public float degreesPerQuad = 6f;
	public float radius = 1000f;
	public float waterHeight = 0f;
	public Material mainMaterial;
	public Material waterMaterial;
	public DisplacementLayer[] displacementLayers;
	public PlanetData() {
		displacementLayers = new DisplacementLayer[0];
	}
}
