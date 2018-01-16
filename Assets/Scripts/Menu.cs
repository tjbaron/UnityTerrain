using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	private int currentShipId = -1;
	private Transform currentShip;
	private Transform[] ships;

	public Transform spaceStation;
	public Transform previewPosition;

	// Use this for initialization
	void Start () {
		ships = Resources.LoadAll<Transform>("SpaceShipsConstructorVol1/Prefabs/");
		NextShip();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void NextShip() {
		if (currentShip != null) Destroy(currentShip.gameObject);
		currentShipId++;
		if (currentShipId >= ships.Length) currentShipId = 0;
		currentShip = (Transform)Instantiate(ships[currentShipId], previewPosition.position, Quaternion.identity);
		currentShip.localScale = new Vector3(0.1f,0.1f,0.1f);
	}

	public void LaunchShip() {
		GetComponent<Canvas>().enabled = false;
		GenerateWorld();
		currentShip.GetComponent<ShipControls>().Launch();
		currentShip.gameObject.AddComponent<SolarObserver>();
		//Camera.main.transform.GetComponent<CameraFollow>().ship = currentShip;
		//Camera.main.transform.GetComponent<CameraController>().SetMode(1);
	}

	void GenerateWorld() {
		//Instantiate(spaceStation, new Vector3(100f,0f,0f), Quaternion.identity);
	}
}
