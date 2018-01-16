using UnityEngine;
using System.Collections;

public class ShipControls : MonoBehaviour {
	bool launched = false;
	float force = 5f;
	bool flymode = false;

	private Vector3 camVelocity = Vector3.zero;
	Rigidbody rb;
	// Use this for initialization
	void Start() {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update() {
		if (launched) {
			var mousepos = 2 * (Input.mousePosition - new Vector3(Screen.width/2f,Screen.height/2f,0f)) / Screen.width;
			var rot = Time.deltaTime * 100 * mousepos;

			if (Input.GetButton("A")) force += 30f*Time.deltaTime;
			if (Input.GetButton("B")) force -= 30f*Time.deltaTime;
			//transform.Rotate(-Input.GetAxis("Vertical")*50f*Time.deltaTime,Input.GetAxis("Horizontal")*50f*Time.deltaTime,Input.GetAxis("Roll")*50f*Time.deltaTime);
			if (mousepos.x > 0.9) force = 1000f*Input.mousePosition.y/(float)Screen.height;
			else if (mousepos.x < -0.9) flymode = mousepos.y>0 ? true : false;
			else if (flymode) transform.Rotate(rot.y,rot.x,0f);
			if (flymode) transform.Translate(0,0,-force*Time.deltaTime);

			var cam = Camera.main.transform;
			Vector3 wantedPos = transform.position + (transform.forward*20f) + (transform.up*4f);
			cam.position = Vector3.SmoothDamp(cam.position, wantedPos, ref camVelocity, 0.2f);
			cam.rotation = transform.rotation;
			cam.Rotate(0,180,0);
		}
	}

	/*void FixedUpdate() {
		if (launched) {
			
			rb.AddForce(-force*transform.forward);
		}
	}*/

	public void Launch() {
		launched = true;
	}
}
