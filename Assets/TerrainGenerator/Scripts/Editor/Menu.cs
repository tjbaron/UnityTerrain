using UnityEditor;
using UnityEngine;
public class MenuTest : MonoBehaviour {
	[MenuItem ("GameObject/Planet", false, 10)]
	static void DoSomething () {
		var go = new GameObject();
		go.name = "Planet";
		var p = go.AddComponent<Planet>();
		p.updateTerrain = true;
	}
}