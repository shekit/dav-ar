using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager> {

	public GameObject marker;
	public GameObject drone;
	public GameObject car;
	private Transform scene;

	Dictionary<int, GameObject> drones = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> cars = new Dictionary<int, GameObject>();

	public Transform[] dronePositions = new Transform[4];
	public Transform[] carPositions = new Transform[3];
	public Transform[] markerPositions = new Transform[3];

	private bool markerCreated = false;

	public void showBids(string type){

		if (type == "drone") {

			for (int i = 0; i < dronePositions.Length; i++) {
				drones [i].SendMessage ("showBid", true);
			}

		} else if (type == "car") {

			for (int i = 0; i < carPositions.Length; i++) {
				cars [i].SendMessage ("showBid", true);
			}
		}
	}

	void Update(){
		if (Input.GetKeyDown ("z")) {
			showBids ("drone");
		}
		if (Input.GetKeyDown ("x")) {
			showBids ("car");
		}
		if (Input.GetKeyDown ("m")) {
			createMarker ();
		}

	}

	public void createMarker(){
		int rnd = Random.Range (0, markerPositions.Length);

		GameObject m = Instantiate (marker, markerPositions [rnd].position, Quaternion.identity);
		m.transform.parent = scene;
		m.transform.localPosition = markerPositions [rnd].localPosition;
		markerCreated = true;
	}

	public void startScene(Transform parent){
		scene = parent;
		for (int i = 0; i < dronePositions.Length; i++) {
			var randomRotation = Quaternion.Euler (0, Random.Range (0, 360), 0);
			GameObject d = Instantiate (drone, dronePositions [i].position, randomRotation);
			d.transform.parent = parent;
			d.transform.localPosition = dronePositions [i].localPosition;
			d.SendMessage ("setId", i);
			drones.Add (i, d);
		}

		for (int i = 0; i < carPositions.Length; i++) {
			var randomRotation = Quaternion.Euler (0, Random.Range (0, 360), 0);
			GameObject c = Instantiate (car, carPositions [i].position, randomRotation);
			c.transform.parent = parent;
			c.transform.localPosition = carPositions [i].localPosition;
			c.SendMessage ("setId", i);
			cars.Add (i, c);
		}
	}

	public void placeVehicle(Vector3 position, string vehicleType, int id){

		var randomRotation = Quaternion.Euler (0, Random.Range (0, 360), 0);
		if (vehicleType == "drone") {
			GameObject d = Instantiate (drone, position, randomRotation);
			d.transform.parent = scene;
			d.transform.localPosition = position;
			drones [id] = d;
			Debug.Log ("Instantiate Drone");
		} else if (vehicleType == "car") {
			GameObject c = Instantiate (car, position, randomRotation);
			drones [id] = c;
			c.transform.parent = scene;
			c.transform.localPosition = position;
			cars [id] = c;
			Debug.Log ("instantiate car");
		}
	}
}
