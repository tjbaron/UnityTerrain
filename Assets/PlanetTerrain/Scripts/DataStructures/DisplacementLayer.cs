using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum NOISE {Perlin,Worley,Texture}

[System.Serializable]
public class DisplacementLayer {
	public NOISE noise;
	public float seed;
	public float height;
	public float detail;
	public AnimationCurve heightStrength;
	public AnimationCurve equatorStrength;
	public Texture2D texture;
	/*public void Save() {
		Stream stream = File.Open(Application.dataPath+"/test", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		//bformatter.Binder = new VersionDeserializationBinder();
		bformatter.Serialize(stream, this);
		stream.Close();
	}*/
}
