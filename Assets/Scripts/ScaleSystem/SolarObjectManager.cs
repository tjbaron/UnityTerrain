using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*	Repositions and resizes celestial objects like suns and planets and nebulas to fit within
	our usable coordinates range. Repositioning happens every frame, so we shouldn't have more
	than a handful of objects using this.
*/

public class SolarObjectManager : MonoBehaviour {

	[System.Serializable]
	public class SolarObject {
		public Vector3d coordinates;
		public Vector3d size;
		public Transform prefab;

		//public double currentDistance;
		//public double maxOffset;
	}

	public Transform selectedObject;
	public SolarObject[] solarObjects;

	Transform observer;
	private double maxDistance = 10000.0;

	// Use this for initialization
	void Start () {
		observer = Camera.main.transform;
		for (int i=0; i<solarObjects.Length; i++) {
			SolarObject o = solarObjects[i];
			o.prefab = (Transform)Instantiate(o.prefab, Vector3.zero, Quaternion.identity);
			if (i==0) {
				// This is where we should actually store selected objects, not in arbitrary scripts like above.
				Global.selectedObject = o.prefab;
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		for (int i=0; i<solarObjects.Length; i++) {

		}
		for (int i=0; i<solarObjects.Length; i++) {
			SolarObject o = solarObjects[i];
			Vector3d observerPosition = new Vector3d(observer.position);
			double dist = Vector3d.Distance(o.coordinates, observerPosition+SolarObserver.offset);
			if (dist < maxDistance) {
				o.prefab.localScale = new Vector3((float)o.size.x, (float)o.size.y, (float)o.size.z);
				// If the object is close put it in its real position
				Vector3d p = o.coordinates-SolarObserver.offset;
				o.prefab.position = new Vector3((float)p.x, (float)p.y, (float)p.z);
			} else {
				// If the object is far projects it at a point on a sphere (centered at the camera)
				// at our maximum workable distance.
				Vector3d dispSize = o.size * maxDistance/dist;
				o.prefab.localScale = new Vector3((float)dispSize.x, (float)dispSize.y, (float)dispSize.z);
				Vector3d newP = ((o.coordinates - (SolarObserver.offset+new Vector3d(observer.position))) * maxDistance/dist) + new Vector3d(observer.position);
				o.prefab.position = new Vector3((float)newP.x, (float)newP.y, (float)newP.z);
			}
			if (i==0) {
				//altitude.text = (dist-o.size.x).ToString();
				Global.targetDistance = dist;
			}
		}
	}
}
