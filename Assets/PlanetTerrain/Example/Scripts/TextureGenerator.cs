using UnityEngine;
using System.Collections;
using System.IO;

public class TextureGenerator : MonoBehaviour {
	void Start () {
		for (var q=0; q<8; q++) {
			Texture2D texture = new Texture2D(128, 128);
			for (var i=0; i<128; i++) {
				for (var j=0; j<128; j++) {
					var v = Noise.Worley(i/8f, j/8f, q/8f);
					texture.SetPixel(i,j,new Color(v,v,v,1));
				}
			}
			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/../PerlinFastest"+q.ToString()+".png", bytes);
		}
	}
}
