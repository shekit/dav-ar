using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager> {

	public GameObject marker;
	public GameObject drone;
	public GameObject car;
	public Transform scene;

	Dictionary<int, GameObject> drones = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> cars = new Dictionary<int, GameObject>();

	public Transform[] dronePositions = new Transform[4];
	public Transform[] carPositions = new Transform[3];
	public Transform[] markerPositions = new Transform[3];

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
