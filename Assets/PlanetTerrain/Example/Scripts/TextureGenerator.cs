using UnityEngine;
using System.Collections;
using System.IO;

public class TextureGenerator : MonoBehaviour {
	void Start () {
		for (var q=0; q<8; q++) {
			Texture2D texture = new Texture2D(128, 128);
			for (var i=0; i<128; i++) {
				for (var j=0; j<128; j++) {
					var v = Noise.Perlin(i/16f, j/16f, q/16f);
					var f = Mathf.Lerp(0.5f,0f, Mathf.Abs(v-0.5f)*20f);
					v = Noise.Perlin(8f+(i/8f), j/8f, q/8f);
					f+= Mathf.Lerp(0.3f,0f, Mathf.Abs(v-0.5f)*10f);
					v = Noise.Perlin(16f+(i/4f), j/4f, q/4f);
					f+= Mathf.Lerp(0.2f,0f, Mathf.Abs(v-0.5f)*5f);
					texture.SetPixel(i,j,new Color(f,f,f,1));
				}
			}
			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/../PerlinMapped"+q.ToString()+".png", bytes);
		}
	}
}
