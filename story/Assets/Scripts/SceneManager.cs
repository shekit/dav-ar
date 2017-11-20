using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager> {

	public GameObject marker;
	public GameObject drone;
	public GameObject car;
	public GameObject ui;
	public GameObject progress;
	private Transform scene;
	private GameObject currentMarker;

	public float showRadarIn = 1.0f;
	public float showBidIn = 2.0f;
	public float selectVehicleIn = 4.5F;


	Dictionary<int, GameObject> drones = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> cars = new Dictionary<int, GameObject>();

	public Transform[] dronePositions = new Transform[4];
	public Transform[] carPositions = new Transform[3];
	public Transform[] markerPositions = new Transform[3];

	private bool markerCreated = false;

	void Awake(){
		showUI (false);
	}

	public void showUI(bool show){
		ui.SetActive (show);
		progress.SetActive (!show);
	}

	public IEnumerator showBids(string type){
		yield return new WaitForSeconds (showBidIn);
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

	public IEnumerator showRadars(string type) {
		yield return new WaitForSeconds (showRadarIn);
		if (type == "drone") {
			for (int i = 0; i < dronePositions.Length; i++) {
				drones [i].SendMessage ("showRadar", true);
			}
		} else if (type == "car") {

			for (int i = 0; i < carPositions.Length; i++) {
				cars [i].SendMessage ("showRadar", true);
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
			createMarker ("drone");
		}
		if (Input.GetKeyDown ("1")) {
			Debug.Log ("one");
			selectVehicle ("drone");
		}
		if (Input.GetKeyDown ("2")) {
			selectVehicle ("car");
		}
	}

	public void createMarker(string type){
		if (markerCreated) {
			// can send back socket message if needed here to notify user
			return;
		}
		showUI (false);
		int rnd = Random.Range (0, markerPositions.Length);

		GameObject m = Instantiate (marker, markerPositions [rnd].position, Quaternion.identity);
		m.transform.parent = scene;
		m.transform.localPosition = markerPositions [rnd].localPosition;
		markerCreated = true;
		currentMarker = m;

		if (type == "drone") {
			currentMarker.SendMessage ("showPackage", true);
		} else if (type == "car") {
			currentMarker.SendMessage ("showPoint", true);
		}

		StartCoroutine (showRadars (type));
		StartCoroutine (showBids (type));
		StartCoroutine (selectVehicle(type));
	}

	public void removePackage(string type){
		if (type == "drone") {
			currentMarker.SendMessage ("showPackage", false);
		} else if (type == "car") {
			currentMarker.SendMessage ("showPoint", false);
		}
	}

	public void destroyMarker(){
		//markerCreated = false;
		Destroy (currentMarker);
	}

	public void changeMarkerCreatedStatus(){
		markerCreated = false;
		showUI (true);
	}
		
	public IEnumerator selectVehicle(string type){
		yield return new WaitForSeconds (selectVehicleIn);
		if (type == "drone") {

			int rnd = Random.Range (0, dronePositions.Length);
			for (int i = 0; i < dronePositions.Length; i++) {
				if (i != rnd) {
					drones [i].SendMessage ("showBid", false);
					drones [i].SendMessage ("showRadar", false);
				}
			}
			drones [rnd].SendMessage ("setTarget", currentMarker.transform);
			drones [rnd].SendMessage ("rotateTowardsTarget");
			drones [rnd].SendMessage ("gotSelected");

		} else if (type == "car") {

			int rnd = Random.Range (0, carPositions.Length);
			for (int i = 0; i < carPositions.Length; i++) {
				if (i != rnd) {
					cars [i].SendMessage ("showBid", false);
					cars [i].SendMessage ("showRadar", false);
				}
			}
			cars [rnd].SendMessage ("setTarget", currentMarker.transform);
			cars [rnd].SendMessage ("rotateTowardsTarget");
			cars [rnd].SendMessage ("gotSelected");

		}
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

		showUI (true);
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
