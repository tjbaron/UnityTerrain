using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	float minutesPerRotation = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0f,minutesPerRotation*360f*Time.deltaTime/60f,0f);
	}
}
