using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Gravity : MonoBehaviour {
	public PlanetTerrain planetScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var h = planetScript.GetHeight(transform.position.normalized);
		if (transform.position.magnitude < h) {
			transform.position = transform.position.normalized * h;
		}

		Vector3 dir = -transform.position.normalized * 9.81f * GetComponent<Rigidbody>().mass;
		GetComponent<Rigidbody>().AddForce(dir.x, dir.y, dir.z);
	}
}
