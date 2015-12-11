using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

	public Transform player;
	public Transform cam;
	private bool flying = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (flying) {
			if (Input.GetKey("space")) {
				transform.Translate(340f*Time.deltaTime,0f,0f);
			} else {
				transform.Translate(75f*Time.deltaTime,0f,0f);
			}
			
			transform.Rotate(0f, (Input.mousePosition.x-(Screen.width/2f))*Time.deltaTime/3f, (Input.mousePosition.y-(Screen.height/2f))*Time.deltaTime/3f);
		
			var r = transform.rotation;
			
			var a = Vector3.Angle(transform.up, transform.position.normalized);
			var n = Vector3.Cross(transform.up, transform.position.normalized);
			transform.RotateAround(transform.position, n, a);

			transform.rotation = Quaternion.Lerp(r, transform.rotation, Time.deltaTime);
		}
	}

	void OnMouseDown() {
		if (!flying) {
			cam.parent = transform;
			Destroy(player.gameObject);
			cam.localPosition = new Vector3(-1000f,0f,0f);
			cam.localEulerAngles = new Vector3(0f,90f,0f);
			gameObject.GetComponent<Gravity>().enabled = false;
			flying = true;
		}
	}
}
