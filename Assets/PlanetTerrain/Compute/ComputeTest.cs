using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeTest : MonoBehaviour {

	public ComputeShader shader;
	public Material mat;

	// Use this for initialization
	void Start () {
		int kernelHandle = shader.FindKernel("CSMain");

		RenderTexture tex = new RenderTexture(512,512,24);
		tex.enableRandomWrite = true;
		tex.Create();
		mat.SetTexture("_MainTex", tex);

		shader.SetTexture(kernelHandle, "Result", tex);
		shader.Dispatch(kernelHandle, 512/8, 512/8, 1);
	}
	
	// Update is called once per frame
	void Update () {
		int kernelHandle = shader.FindKernel("CSMain");
		shader.SetFloat("_Time", Time.time);
		shader.Dispatch(kernelHandle, 512/8, 512/8, 1);
	}
}
