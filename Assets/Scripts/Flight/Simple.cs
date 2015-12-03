using UnityEngine;
using System.Collections;

public class Simple : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(0f,0f,Time.deltaTime*.5f);
		transform.Rotate(-Input.GetAxis("Vertical")*30f*Time.deltaTime,Input.GetAxis("Horizontal")*30f*Time.deltaTime,0f);
		//var cr = transform.rotation;
		//transform.LookAt(new Vector3(0f,0f,0f));
		//transform.Rotate(-90f,0f,0f);
		//transform.rotation = Quaternion.Lerp(cr, transform.rotation, Time.deltaTime/5f);
	}
}
