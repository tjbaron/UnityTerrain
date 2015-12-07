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
		p.mainMaterial = mat;
		mat.SetTexture("_MainTex", mainTexture);
		mat.SetTexture("_OuterTex", mountainTexture);
		mat.SetTexture("_PoleTex", poleTexture);
		mat.SetVector("_CentrePoint", new Vector4(transform.position.x, transform.position.y, transform.position.z));
		mat.SetFloat("_ChangePoint", p.radius+30f);
		mat.SetFloat("_EquatorWidth", p.radius/2f);
	}
}
