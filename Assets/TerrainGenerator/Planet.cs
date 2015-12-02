using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	public int segmentResolution = 32;
	public float radius = 32f;
	public Material material;
	public Material waterMaterial;

	// Use this for initialization
	void Start () {
		MakeSphere(true, radius);
		MakeSphere(false, radius+6f);
	}

	void MakeSphere(bool displace, float rad) {
		for (var i=0; i<6; i++) {
			var go = new GameObject();
			var mf = go.AddComponent<MeshFilter>();
			var mr = go.AddComponent<MeshRenderer>();
			var mc = go.AddComponent<MeshCollider>();
			if (i == 0) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,-0.5f,0.5f), 
				new Vector3(0.5f,-0.5f,0.5f),
				new Vector3(-0.5f,0.5f,0.5f),
				new Vector3(0.5f,0.5f,0.5f), displace);
			} else if (i == 1) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,0.5f,0.5f), 
				new Vector3(0.5f,0.5f,0.5f),
				new Vector3(-0.5f,0.5f,-0.5f),
				new Vector3(0.5f,0.5f,-0.5f), displace);
			} else if (i == 2) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,-0.5f,-0.5f), 
				new Vector3(0.5f,-0.5f,-0.5f),
				new Vector3(-0.5f,-0.5f,0.5f),
				new Vector3(0.5f,-0.5f,0.5f), displace);
			} else if (i == 3) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(0.5f,-0.5f,-0.5f), 
				new Vector3(0.5f,0.5f,-0.5f),
				new Vector3(0.5f,-0.5f,0.5f),
				new Vector3(0.5f,0.5f,0.5f), displace);
			} else if (i == 4) {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(-0.5f,-0.5f,0.5f), 
				new Vector3(-0.5f,0.5f,0.5f),
				new Vector3(-0.5f,-0.5f,-0.5f),
				new Vector3(-0.5f,0.5f,-0.5f), displace);
			} else {
				mf.sharedMesh = Segment.Generate(segmentResolution, rad, 
				new Vector3(0.5f,-0.5f,-0.5f), 
				new Vector3(-0.5f,-0.5f,-0.5f),
				new Vector3(0.5f,0.5f,-0.5f),
				new Vector3(-0.5f,0.5f,-0.5f), displace);
			}
			if (displace) mr.sharedMaterial = material;
			else mr.sharedMaterial = waterMaterial;
			mc.sharedMesh = mf.sharedMesh;
			go.GetComponent<Transform>().parent = transform;
		}
	}
}
