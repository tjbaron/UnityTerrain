using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 dir = -transform.position.normalized * 9.81f * GetComponent<Rigidbody>().mass;
		GetComponent<Rigidbody>().AddForce(dir.x, dir.y, dir.z);
	}
}
