using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class PlaceMessage : Singleton<PlaceMessage> {

	public GameObject startScene;
	private GameObject scene;

	public GameObject markerPrefab;
	private GameObject marker;

	private bool msgIsCreated = false;
	private bool planeFound = false;
	private bool markerIsCreated = false;

	private Vector3 startPoint;

	private float sceneSize = 1.0f;
	private float sceneRotation = 0.0f;

	// Use this for initialization
	void Start () {
		UnityARSessionNativeInterface.ARAnchorAddedEvent += AnchorAdded;
	}
	
	void CreateMessage(GameObject go, Vector3 position){
		msgIsCreated = true;
		scene = Instantiate (go, position, Quaternion.identity);
		if (markerIsCreated) {
			Destroy (marker);
			markerIsCreated = false;
		}
	}

	public void DestroyScene(){
		if (scene) {
			Destroy (scene);
		}
	}

	public void DestroyMarker(){
		if (markerIsCreated) {
			Destroy (marker);
			markerIsCreated = false;
		}
	}

	private void AnchorAdded(ARPlaneAnchor arPlaneAnchor){
		planeFound = true;
	}

	void CreateCrossHair(Vector3 position, Quaternion rotation){
		if (!markerIsCreated) {
			markerIsCreated = true;
			marker = Instantiate (markerPrefab, position, rotation);
		} else {
			marker.transform.position = position;
			marker.transform.rotation = rotation;
		}

		startPoint = position;
	}

	public void Reset(){
		DestroyScene ();
		DestroyMarker ();
		msgIsCreated = false;
	}

	public void RotateScene(float val){
		Debug.Log (val);
		if (msgIsCreated) {
			scene.transform.rotation = Quaternion.Euler (0, val * 360, 0);
			sceneRotation = val;
		}
	}

	public void ResizeScene(float val){
		if (msgIsCreated) {
			scene.transform.localScale = new Vector3 (val, val, val);
			sceneSize = val;
		}
	}

	public void createMessageTest(){
		Vector3 pos = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y-.4f,
			Camera.main.transform.position.z + 1.5f);

		CreateMessage (startScene, pos);
	}

	void Update() {
		if (planeFound && !msgIsCreated) {
			RaycastHit hit;

			Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5F, 0.5F, 0));

			if (Physics.Raycast (ray, out hit)) {
				CreateCrossHair (hit.point, hit.transform.rotation);
			}
		}

		if (Input.touchCount > 0) {
			var touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began) {
				if (!msgIsCreated && markerIsCreated) {
					CreateMessage (startScene, startPoint);
				}
			}
		}

		if (Input.GetKeyDown ("space")) {
			createMessageTest ();
		}
	}
}
