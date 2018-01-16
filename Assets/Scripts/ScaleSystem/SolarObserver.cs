using UnityEngine;
using System.Collections;

public class SolarObserver : MonoBehaviour {

	public static Vector3d offset;

	public Transform[] ships;

	public static double maxOffset;

	// Use this for initialization
	void Start () {
		maxOffset = 100000.0f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3d transformPosition = new Vector3d(transform.position);
		Vector3d no = new Vector3d();

		if (Mathd.Abs(transformPosition.x) > maxOffset) {
			no.x += Mathd.Floor(transformPosition.x/maxOffset)*Mathd.Abs(maxOffset);
		}
		if (Mathd.Abs(transformPosition.y) > maxOffset) {
			no.y += Mathd.Floor(transformPosition.y/maxOffset)*Mathd.Abs(maxOffset);
		}
		if (Mathd.Abs(transformPosition.z) > maxOffset) {
			no.z += Mathd.Floor(transformPosition.z/maxOffset)*Mathd.Abs(maxOffset);
		}

		offset += no;

		Vector3 nof = new Vector3((float)no.x, (float)no.y, (float)no.z);
		for (var i=0; i<ships.Length; i++) {
			ships[i].position -= nof;
		}
		transform.position -= nof;
	}

	void OnGUI() {
		GUILayout.Label(offset.x.ToString());
		GUILayout.Label(offset.y.ToString());
		GUILayout.Label(offset.z.ToString());
	}
}
