using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeTest2 : MonoBehaviour {

	public ComputeShader shader;
	public Material mat;
	RenderTexture tex;
	public MeshFilter mf;

	Vector3[] newVertices = new Vector3[128*128];
	ComputeBuffer buffer;

	// Use this for initialization
	void Start () {
		int kernelHandle = shader.FindKernel("CSMain");

		/*tex = new RenderTexture(128,128,24);
		tex.enableRandomWrite = true;
		tex.Create();
		mat.SetTexture("_MainTex", tex);*/

		//shader.SetTexture(kernelHandle, "Result", tex);

		buffer = new ComputeBuffer(newVertices.Length, 12);
		buffer.SetData(newVertices);
		shader.SetBuffer(kernelHandle, "Result", buffer);

		shader.Dispatch(kernelHandle, 128/8, 128/8, 1);
	}
	
	public static Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

	// Update is called once per frame
	void Update () {
		int kernelHandle = shader.FindKernel("CSMain");
		shader.SetFloat("_Time", Time.time);
		shader.Dispatch(kernelHandle, 128/8, 128/8, 1);

		buffer.GetData(newVertices);

		int res = 127;
		
		Vector3[] newNormals = new Vector3[128*128];
		Vector2[] newUV = new Vector2[128*128];
		int[] newTriangles = new int[res*res*3*2];
		for (int y=0; y<res+1; y++) {
			for (int x=0; x<res+1; x++) {
				var v = (y*(res+1))+x;

				//newVertices[v] = new Vector3(x, y, 0f);
				newUV[v] = new Vector2(Mathf.Lerp(0f, 1f, x/(float)res), Mathf.Lerp(0f, 1f, y/(float)res));

				if (x > 0 && y > 0) {
					var i = ((x-1)*res*6)+((y-1)*6);
					newTriangles[i] = v;
					newTriangles[i+1] = v-1;
					newTriangles[i+2] = v-res-1;
					newTriangles[i+3] = v-1;
					newTriangles[i+4] = v-res-2;
					newTriangles[i+5] = v-res-1;
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		mesh.uv = newUV;
		mesh.RecalculateNormals();
		mf.mesh = mesh;
	}
}
