using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateStars : MonoBehaviour {

	public Sprite sprite;
	public TextAsset stardata;

	SgtCustomStarfield sgt;

	private string[] lines;

	// Use this for initialization
	void Start () {
		lines = stardata.text.Split('\n');

		sgt = transform.GetComponent<SgtCustomStarfield>();
		//sgt.Stars = 
		List<SgtStarfieldStar> stars = new List<SgtStarfieldStar>();

		for (int i=1; i<lines.Length; i++) {
			string[] line = lines[i].Split(',');
			if (line.Length > 9) {
				float mag = float.Parse(line[14]);
				if (mag < 3.0f) {
					float specid = 0.0f;
					string spec = line[15];
					float ra = float.Parse(line[7]);
					float dec = float.Parse(line[8]);
					float dist = float.Parse(line[9]);
					float brightness = 0.8f/Mathf.Pow(1.4f,mag);//1/Mathf.Pow(2.512f,mag);
					float blue = 0.0f;
					float red = 0.0f;
					float green = 0.0f;
					float radius = 15.0f * brightness;
					if (radius/dist > 0.3f) radius = 0.3f*dist;
					Quaternion rotation = Quaternion.identity;
					rotation.eulerAngles = new Vector3(-dec, -ra*15, 0);
					
					if (dist > 1000) continue;

					/*if (line[2] == "8890") {
						red *= 0.0f;
						green = 0.0f;
						blue = 3.0f;
					}*/

					if (spec.Length == 0) {
						specid = 0.0f;
					} else if (spec.StartsWith("O")) {
						specid = -1.0f;
					} else if (spec.StartsWith("B")) {
						specid = -0.75f;
					} else if (spec.StartsWith("A")) {
						specid = -0.5f;
					} else if (spec.StartsWith("F")) {
						specid = -0.25f;
					} else if (spec.StartsWith("G")) {
						specid = 0.0f;
					} else if (spec.StartsWith("K")) {
						specid = 0.25f;
					} else if (spec.StartsWith("M")) {
						specid = 0.5f;
					} else if (spec.StartsWith("L")) {
						specid = 0.75f;
					} else if (spec.StartsWith("T")) {
						specid = 1.0f;
					}
					specid /= 2;

					if (specid < 0) {
						specid = Mathf.Abs(specid);
						blue = 1.0f*brightness;
						red = green = (1.0f-specid)*brightness;
					} else {
						red = 1.0f*brightness;
						blue = green = (1.0f-specid)*brightness;
					}

					// Pliedes
					/*if (line[2] == "17702" || line[2] == "23850" || line[2] == "23302" || line[2] == "23408" || line[2] == "23480") {
						red *= 3.0f;
						green = 0.0f;
						blue = 0.0f;
					}*/

					// North Star
					//if (line[2] == "128620") {

					// Big Dipper
					/*if (line[2] == "95689" || line[2] == "95418" || line[2] == "103287" || line[2] == "106591" || line[2] == "112185" || line[2] == "116656" || line[2] == "120315") {
						red = 0.0f;
						green *= 3.0f;
						blue = 0.0f;
					}*/

					// Orion's Belt
					/*if (line[2] == "37742" || line[2] == "37128" || line[2] == "36486" || line[2] == "36485") {
						red = 0.0f;
						green *= 3.0f;
						blue = 0.0f;
					}*/

					SgtStarfieldStar star = new SgtStarfieldStar();
					star.Sprite = sprite;
					star.Radius = radius;
					star.Color = new Color(red,green,blue,1.0f);
					star.Position = rotation*Vector3.forward*10*dist;
					stars.Add(star);
				}
			}
		}

		sgt.Stars = stars;
	}

}
