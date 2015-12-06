using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Gravity))]
public class OurController : MonoBehaviour {
	public PlanetTerrain planetScript;

	void Update () {
		var a = Vector3.Angle(transform.up, transform.position.normalized);
		var n = Vector3.Cross(transform.up, transform.position.normalized);
		transform.RotateAround(transform.position, n, a);
		transform.Translate(Input.GetAxis("Horizontal")*Time.deltaTime*20f, 0f, Input.GetAxis("Vertical")*Time.deltaTime*20f);
		transform.Rotate(0f, Input.GetAxis("Mouse X")*Time.deltaTime*100f, 0f);

		var h = planetScript.GetHeight(transform.position.normalized);
		if (transform.position.magnitude < h) {
			transform.position = transform.position.normalized * h;
		}
	}
	void FixedUpdate() {
		if (Input.GetKeyDown("space")) GetComponent<Rigidbody>().AddForce(transform.position.normalized*100f);
	}
}
