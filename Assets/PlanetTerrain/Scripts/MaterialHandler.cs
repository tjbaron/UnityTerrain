using UnityEngine;
using System.Collections;

[System.Serializable]
public class MaterialHandler {
	private Material mat;
	public Texture2D mainTexture;
	public Texture2D heightTexture;
	public Texture2D skyTexture;
	public Texture2D mountainTexture;
	public Texture2D poleTexture;

	private Material waterMat;

	public float textureTransition = 0f;
	
	public void Refresh(Transform transform, PlanetData p) {
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
		if (mat == null) {
			mat = new Material(Shader.Find("Terrain/HeightTexture"));
		}
		waterMat = p.waterMaterial;

		var maxHeight = 0f;
		for (var i=0; i<p.displacementLayers.Length; i++) {
			var l = p.displacementLayers[i];
			maxHeight += l.height;
		}
		var radius = p.radius * transform.lossyScale.x;
		maxHeight *= transform.lossyScale.x;

		p.mainMaterial = mat;
		mat.SetTexture("_MainTex", mainTexture);
		mat.SetTexture("_HeightTex", heightTexture);
		mat.SetTexture("_SkyTex", skyTexture);
		mat.SetTexture("_OuterTex", mountainTexture);
		mat.SetTexture("_PoleTex", poleTexture);
		mat.SetTextureScale("_MainTex", new Vector2(1000f,1000f));
		mat.SetTextureScale("_OuterTex", new Vector2(1000f,1000f));
		mat.SetTextureScale("_PoleTex", new Vector2(1000f,1000f));
		mat.SetVector("_CentrePoint", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f));
		mat.SetFloat("_ChangePoint", textureTransition);//radius+(maxHeight*textureTransition));
		mat.SetFloat("_EquatorWidth", radius/2f);

		if (waterMat != null) {
			waterMat.SetTextureOffset("_BumpMap", new Vector2(Time.time/10f, 0f));
		}
	}
}
