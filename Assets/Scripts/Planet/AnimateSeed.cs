using UnityEngine;
using System.Collections;

public class AnimateSeed : MonoBehaviour {
	public Planet planet;
	private float lastChange = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lastChange += Time.deltaTime;
		if (lastChange > 0.3f) {
			planet.displacementLayers[0].seed += 0.05f;
			planet.updateTerrain = true;
			lastChange = 0.0f;
		}
	}
}
