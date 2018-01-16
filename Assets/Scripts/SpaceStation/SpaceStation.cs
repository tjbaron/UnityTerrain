using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceStation : MonoBehaviour {
	public Transform explosion;
	public Transform[] components;
	public int activeComponent = 0;
	public List<Transform> parts = new List<Transform>();

	// Use this for initialization
	void Start () {
		components = Resources.LoadAll<Transform>("SpaceStation/");
	}
	
	// Update is called once per frame
	void Update () {
		activeComponent = (int)Mathf.Floor(Random.Range(0.5f,((float)components.Length)-0.001f));
		if (Input.GetButtonDown("X")) activeComponent--;
		if (Input.GetButtonDown("Y")) activeComponent++;
		if (Input.GetKeyDown("`")) activeComponent=0;
		if (Input.GetKeyDown("1")) activeComponent=1;
		if (Input.GetKeyDown("2")) activeComponent=2;
		if (Input.GetKeyDown("3")) activeComponent=3;
		if (Input.GetKeyDown("4")) activeComponent=4;
		if (Input.GetKeyDown("5")) activeComponent=5;
		if (Input.GetKeyDown("6")) activeComponent=6;
		if (Input.GetButtonDown("L1") && parts.Count>0) {
			DestroyImmediate(parts[parts.Count-1].gameObject);
			parts.RemoveAt(parts.Count-1);
		}
		if (activeComponent >= components.Length) activeComponent--;
		if (activeComponent < 0) activeComponent++;
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500, 1<<LayerMask.NameToLayer("SpaceStation"))) {
				var mnts = hit.collider.gameObject.GetComponentsInChildren<Transform>();
				var m = mnts[0];
				var d = Vector3.Distance(hit.point, m.position);
				for (int i=1; i<mnts.Length; i++) {
					var c = mnts[i];
					var nd = Vector3.Distance(hit.point, c.position);
					if (nd < d) {
						m = c;
						d = nd;
					}
				}
				var comp = components[activeComponent];
				var inst = (Transform)Instantiate(comp, Vector3.zero, m.rotation);
				parts.Add(inst);
				inst.gameObject.layer = 8;
				inst.position = m.position-inst.FindChild("Marker.2").transform.position;
				inst.parent = transform;
			}
		}
		if (Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 500)) {
				var go = hit.collider.gameObject;
				Instantiate(explosion, go.transform.position, Quaternion.identity);
				Destroy(go);
			}
		}
	}
}
