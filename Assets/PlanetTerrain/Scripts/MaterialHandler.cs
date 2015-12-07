using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MaterialHandler : MonoBehaviour {
	private Material mat;
	public Texture2D mainTexture;
	public Texture2D mountainTexture;
	public Texture2D poleTexture;

	private PlanetData p;

	// Use this for initialization
	void Start () {
		mat = new Material(Shader.Find("Terrain/HeightTexture"));
	}
	
	// Update is called once per frame
	void Update () {
		p = gameObject.GetComponent<PlanetTerrain>().planet;

		var maxHeight = 0f;
		for (var i=0; i<p.displacementLayers.Length; i++) {
			var l = p.displacementLayers[i];
			maxHeight += l.height;
		}

		p.mainMaterial = mat;
		mat.SetTexture("_MainTex", mainTexture);
		mat.SetTexture("_OuterTex", mountainTexture);
		mat.SetTexture("_PoleTex", poleTexture);
		mat.SetTextureScale("_MainTex", new Vector2(100f,100f));
		mat.SetTextureScale("_OuterTex", new Vector2(100f,100f));
		mat.SetTextureScale("_PoleTex", new Vector2(100f,100f));
		mat.SetVector("_CentrePoint", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f));
		mat.SetFloat("_ChangePoint", p.radius+(maxHeight/2f));
		mat.SetFloat("_EquatorWidth", p.radius/2f);
	}
}
