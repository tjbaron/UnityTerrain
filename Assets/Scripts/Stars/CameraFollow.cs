using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	SgtObserver ob;

	void Start () {
		ob = GetComponent<SgtObserver>();
	}

	void Update() {
		var tp = transform.position;
		ob.StarfieldPosition = new Vector3(tp.x-3000, tp.y-3000, tp.z+12000)/50f;
	}
}
