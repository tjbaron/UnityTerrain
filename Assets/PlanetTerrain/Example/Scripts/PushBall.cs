using UnityEngine;
using System.Collections;

public class PushBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		GetComponent<Rigidbody>().AddTorque(Input.GetAxis("Horizontal")*Vector3.Cross(transform.position.normalized, Camera.main.transform.right).normalized*100f);
		GetComponent<Rigidbody>().AddTorque(Input.GetAxis("Vertical")*Vector3.Cross(transform.position.normalized, Camera.main.transform.up).normalized*100f);
		if (Input.GetKeyDown("space")) {
			GetComponent<Rigidbody>().AddForce(transform.position.normalized*100f);
		}
	}
}
