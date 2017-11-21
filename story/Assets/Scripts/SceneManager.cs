using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager> {

	public GameObject marker;
	public GameObject drone;
	public GameObject car;
	public GameObject ui;
	public GameObject progress;
	public GameObject sliders;
	public GameObject introCanvas;

	private bool slidersOn = false;

	private Transform scene;
	private GameObject currentMarker;

	public float showRadarIn = 1.0f;
	public float showBidIn = 2.0f;
	public float selectVehicleIn = 4.5F;


	Dictionary<int, GameObject> drones = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> cars = new Dictionary<int, GameObject>();

	public Transform[] dronePositions = new Transform[4];
	public Transform[] carPositions = new Transform[4];
	public Transform[] markerPositions = new Transform[10];

	private bool markerCreated = false;



	void Awake(){
		ui.SetActive (false);
		sliders.SetActive (false);
		progress.SetActive (false);
	}

	public void showUI(bool show){
		ui.SetActive (show);
		progress.SetActive (!show);
	}

	public IEnumerator showBids(string type){
		yield return new WaitForSeconds (showBidIn);
		setProgressStatus ("Autonomous Bidding");
		if (type == "drone") {

			for (int i = 0; i < dronePositions.Length; i++) {
				if (drones [i] != null) {
					drones [i].SendMessage ("showBid", true);
				} else {
					Debug.Log ("ERROR TO EXISTING DRONE - SHOW BID");
					//setProgressStatus ("Error Bidding - Please try again");
					//StartCoroutine ("unknownError");
				}
			}

		} else if (type == "car") {

			for (int i = 0; i < carPositions.Length; i++) {
				if (cars [i] != null) {
					cars [i].SendMessage ("showBid", true);
				} else {
					Debug.Log ("ERROR TO EXISTING CAR - SHOW BID");
					//setProgressStatus ("Error Bidding - Please try again");
					//StartCoroutine ("unknownError");
				}
			}
		}
	}

	public void setProgressStatus(string s){
		progress.SendMessage("setStatus", s);
	}

	public IEnumerator showRadars(string type) {
		yield return new WaitForSeconds (showRadarIn);
		setProgressStatus ("Vehicles Notified");
		if (type == "drone") {
			for (int i = 0; i < dronePositions.Length; i++) {
				if (drones [i] != null) {
					drones [i].SendMessage ("showRadar", true);
				} else {
					Debug.Log ("ERROR TO EXISTING DRONE - SHOW RADAR " + i.ToString());
					//setProgressStatus ("Error Notifying - Please try again");
					//StartCoroutine ("unknownError");
				}
			}
		} else if (type == "car") {

			for (int i = 0; i < carPositions.Length; i++) {
				if (cars [i] != null) {
					cars [i].SendMessage ("showRadar", true);
				} else {
					Debug.Log ("ERROR TO EXISTING CAR - SHOW RADAR " + i.ToString());
					//setProgressStatus ("Error Notifying - Please try again");
					//StartCoroutine ("unknownError");
				}
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

	public void toggleSliders(){
		slidersOn = !slidersOn;
		sliders.SetActive (slidersOn);
	}

	public void createMarker(string type){
		if (markerCreated) {
			// can send back socket message if needed here to notify user
			return;
		}
		showUI (false);
		setProgressStatus ("Request Received");
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

	IEnumerator unknownError(){
		yield return new WaitForSeconds (1.5F);
		changeMarkerCreatedStatus ();
	}

	public void changeMarkerCreatedStatus(){
		markerCreated = false;
		StartCoroutine ("delayUIShow");
	}

	IEnumerator delayUIShow(){
		yield return new WaitForSeconds (1.3f);
		showUI (true);
	}
		
	public IEnumerator selectVehicle(string type){
		yield return new WaitForSeconds (selectVehicleIn);
		setProgressStatus ("AV selected");
		if (type == "drone") {

			int rnd = Random.Range (0, dronePositions.Length);
			for (int i = 0; i < dronePositions.Length; i++) {
				if (i != rnd) {
					if (drones [i] != null) {
						drones [i].SendMessage ("showBid", false);
						drones [i].SendMessage ("showRadar", false);
					} else {
						Debug.Log ("ERROR TO EXISTING DRONE - SELECT VEHICLE");
						setProgressStatus ("Drone Failure - Try again");
						StartCoroutine ("unknownError");
						yield break;
					}
				}
			}
			if (drones [rnd] != null) {
				drones [rnd].SendMessage ("setTarget", currentMarker.transform);
				drones [rnd].SendMessage ("rotateTowardsTarget");
				drones [rnd].SendMessage ("gotSelected");
			} else {
				Debug.Log("ERROR TO SELECTED DRONE - SELECT-VEHICLE");
				setProgressStatus ("Drone Failure - Try again");
				StartCoroutine ("unknownError");
				yield break;
			}

		} else if (type == "car") {
			Debug.Log ("CAR LENGTH "+carPositions.Length.ToString());
			int rnd = Random.Range (0, carPositions.Length);
			Debug.Log ("SELECTED CAR " + rnd.ToString ());
			for (int i = 0; i < carPositions.Length; i++) {
				if (i != rnd) {
					if (cars [i] != null) {
						cars [i].SendMessage ("showBid", false);
						cars [i].SendMessage ("showRadar", false);
					} else {
						Debug.Log ("ERROR TO EXISTING CAR - SELECT VEHICLE");
						setProgressStatus ("Car Failure - Try again");
						StartCoroutine ("unknownError");
						yield break;
					}
				}
			}
			if (cars [rnd] != null) {
				cars [rnd].SendMessage ("setTarget", currentMarker.transform);
				cars [rnd].SendMessage ("rotateTowardsTarget");
				cars [rnd].SendMessage ("gotSelected");
			} else {
				Debug.Log("ERROR TO SELECTED CAR - SELECT-VEHICLE");
				setProgressStatus ("Car Failure - Try again");
				StartCoroutine ("unknownError");
				yield break;
			}

		}
	}

	public void startScene(Transform parent){
		introCanvas.SetActive (false);
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
		Debug.Log ("ID RECEIVED " + id.ToString ());
		var randomRotation = Quaternion.Euler (0, Random.Range (0, 360), 0);
		if (vehicleType == "drone") {
			GameObject d = Instantiate (drone, position, randomRotation);
			d.transform.parent = scene;
			d.transform.localPosition = position;
			d.SendMessage ("setId", id);
			d.SendMessage ("setScale");
			drones [id] = d;
			Debug.Log ("Instantiate Drone " + id.ToString());
		} else if (vehicleType == "car") {
			GameObject c = Instantiate (car, position, randomRotation);
			c.transform.parent = scene;
			c.transform.localPosition = position;
			c.SendMessage ("setId", id);
			c.SendMessage ("setScale");
			cars [id] = c;
			Debug.Log ("Instantiate car " + id.ToString());
		}
	}
}
