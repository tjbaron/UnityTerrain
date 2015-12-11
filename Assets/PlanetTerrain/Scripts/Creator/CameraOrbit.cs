using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {
	public Transform planet;
	private PlanetData ps;
	private Camera cam;
	private float distance = 1000f;
	private float rotateSpeed = 50f;
	
	void Start() {
		ps = planet.GetComponent<PlanetTerrain>().planet;
		cam = gameObject.GetComponent<Camera>();
	}

	void Update () {
		if (Input.GetKeyUp("left shift")) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} else if (Input.GetKey("left shift")) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Mouse X")*Time.deltaTime*rotateSpeed);
			transform.RotateAround(Vector3.zero, transform.right, -Input.GetAxis("Mouse Y")*Time.deltaTime*rotateSpeed);
		}
		if (Input.GetKey("x")) {
			distance *= 0.99f;
		} else if (Input.GetKey("z")) {
			distance *= 1.01f;
			if (ps.radius+distance+cam.nearClipPlane > cam.farClipPlane) {
				distance = cam.farClipPlane-ps.radius-cam.nearClipPlane;
			}
		}
		transform.position = transform.position.normalized*(ps.radius+distance+cam.nearClipPlane);
	}
}
