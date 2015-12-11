using UnityEngine;
using System.Collections;
using System.IO;

public class PlanetPaint : MonoBehaviour {
	public Transform planet;
	public Texture2D[] brushes;
	private Texture2D tex;
	private Camera cam;
	private PlanetTerrain ps;
	private float[] brush;
	private float strength = 0.5f;
	private int size = 32;

	void Start () {
		tex = new Texture2D(512,512);
		
		var c = new Color(0f,0f,0f,1f);
		var nc = new Color[262144];
		for (var i=0; i<262144; i++) {
			nc[i] = c;
		}
		tex.SetPixels(nc);

		cam = gameObject.GetComponent<Camera>();
		ps = planet.GetComponent<PlanetTerrain>();
		brush = new float[(int)Mathf.Pow(size,2)];
		for (var i=0; i<size*size; i++) {
			var x = i%size;
			var y = Mathf.Floor(i/size);
			brush[i] = brushes[1].GetPixelBilinear((float)x/(float)size,(float)y/(float)size).grayscale;
		}
		ps.planet.displacementLayers = new DisplacementLayer[]{
			new DisplacementLayer()
		};
		var l = ps.planet.displacementLayers[0];
		l.noise = NOISE.Texture;
		l.height = 50f;
		l.texture = tex;
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, cam.farClipPlane)) {
				//Debug.Log(hit.point);
				var tc = PTHelpers.GetHeightmapCoord(hit.point);
				var x = (int)Mathf.Clamp(Mathf.Floor(tc.x*512) - 16, 0f, 480f);
				var y = (int)Mathf.Clamp(Mathf.Floor(tc.y*512) - 16, 0f, 480f);
				var cur = tex.GetPixels(
					x,
					y,
					32,
					32);
				for (var i=0; i<1024; i++) {
					cur[i].r = cur[i].g = cur[i].b = Mathf.Clamp(cur[i].r+(brush[i]*strength), 0f, 1f);
				}
				tex.SetPixels(
					x,
					y,
					32,
					32,
					cur);
				byte[] bytes = tex.EncodeToPNG();		
				File.WriteAllBytes(Application.dataPath + "/PlanetTex.png", bytes);
				ps.UpdateTerrain();
			}
		}
	}

	void OnGUI() {
		GUI.Window(0, new Rect(0,0,200,500), BrushControls, "Brush Controls");
		GUI.Window(1, new Rect(0,Screen.height-100,200,100), Instructions, "Controls");
	}

	void BrushControls(int windowID) {
		GUILayout.Label("Brush Strength");
		strength = GUILayout.HorizontalSlider(strength, 0.01f, 1f);
		GUILayout.Label("Brush Size");
		//size = GUILayout.HorizontalSlider(size, 1, 500);
	}

	void Instructions(int windowID) {
		GUILayout.Label("Rotate - Left Shift + Mouse");
		GUILayout.Label("Zoom In/Out - X and Z");
	}
}
