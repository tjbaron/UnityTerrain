using UnityEditor;
using UnityEngine;
public class MenuTest : MonoBehaviour {
	[MenuItem ("GameObject/Planet Terrain", false, 10)]
	static void DoSomething () {
		var go = new GameObject();
		go.name = "Planet Terrain";
		var p = go.AddComponent<PlanetTerrain>();
		p.UpdateTerrain();
	}
}