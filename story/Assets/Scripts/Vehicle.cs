﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

	private GameObject radar;
	private GameObject bid;
	private GameObject package;
	private TextMesh bidValue;
	private Animator anim;
	public Transform targett;
	private Transform target;
	private Vector3 startPosition;
	private Vector3 localStartPosition;

	private bool rotateToTarget = false;
	private bool goAway = false;
	private bool startMoving = false;
	private bool targetSet = false;
	private bool startRising = false;

	public float rotateSpeed = 2.0f;
	public float rotateError = 0.5f;
	public float riseSpeed = 0.2F;
	public float riseHeight = 0.1f;
	public float arriveSpeed = 0.2F;
	public float leaveSpeed = 0.3F;
	public float distance = 0.001F;
	public float waitAtTarget = 1.5F;
	public float waitToDestroy = 4.0F;

	public int minBid = 18;
	public int maxBid = 27;

	private int id;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		radar = gameObject.transform.Find ("ring").gameObject;
		bid = gameObject.transform.Find ("bid").gameObject;
		bidValue = bid.GetComponentsInChildren<TextMesh> () [0];
		startPosition = transform.position;
		localStartPosition = transform.localPosition;
		showRadar (false);
		showBid (false);

		if (tag == "drone") {
			package = gameObject.transform.Find ("package").gameObject;
			showPackage (false);
		}
		//setTarget (targett); // FOR TESTING, OTHERWISE TARGET WILL BE ASSIGNED
	}

	public void setId(int num){
		id = num;
	}
	
	// Update is called once per frame
	public void setTarget(Transform t){
		//Debug.Log ("SET TARGET");
		target = t;
		targetSet = true;
	}

	private void rotateBidToCamera(){
		var bidPos = Camera.main.transform.position - bid.transform.position;
		//bidPos.y = 0;
		var rotation = Quaternion.LookRotation(bidPos);
		bid.transform.rotation = rotation;
	}
		
	public void rotateTowardsTarget(){
		if (!targetSet) {
			Debug.Log ("NO TARGET");
			return;
		} 

		rotateToTarget = true;

		// vector from current position to target
		Vector3 dir = target.position - transform.position;
		// prevent it from tilting up or down
		dir.y = 0;

		//create the rotation needed to look at the target
		Quaternion rotation = Quaternion.LookRotation (dir);

		// rotate over time according to speed till you reach reqd rotation
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, rotateSpeed * Time.deltaTime);

		// stop rotation when certain threshold of difference is met
		if (Quaternion.Angle (rotation, transform.rotation) <= rotateError) {
			//Debug.Log ("Done");
			rotateToTarget = false;

			if (gameObject.tag == "drone") {
				StartCoroutine ("riseCoroutine");
				anim.SetBool ("rotors", true);
			} else if (gameObject.tag == "car") {
				anim.SetBool ("rotors", true);
				StartCoroutine ("moveCoroutine");
			}
			SceneManager.Instance.setProgressStatus ("Trip in Progress");
		}
	}

	IEnumerator riseCoroutine(){
		yield return new WaitForSeconds (1.0F);
		startRising = true;
	}

	IEnumerator moveCoroutine(){
		yield return new WaitForSeconds (1.0F);
		startMoving = true;
	}

	IEnumerator goawayCoroutine(){
		yield return new WaitForSeconds (waitAtTarget);
		if (tag == "drone") {
			showPackage (true);
		}
		SceneManager.Instance.removePackage (tag);
		goAway = true;
		SceneManager.Instance.setProgressStatus ("Package Collected");
		StartCoroutine ("destroyMarker");
		StartCoroutine ("destroyObject");
	}

	IEnumerator destroyMarker(){
		yield return new WaitForSeconds (1.0F);
		SceneManager.Instance.destroyMarker ();
	}

	IEnumerator destroyObject(){
		yield return new WaitForSeconds (waitToDestroy);
		SceneManager.Instance.setProgressStatus ("Package Delivered");
		SceneManager.Instance.placeVehicle (localStartPosition, gameObject.tag, id);
		SceneManager.Instance.changeMarkerCreatedStatus ();
		Destroy (gameObject);
	}

	public void moveToTarget(Transform target){
		Vector3 start = new Vector3 (transform.position.x, 0, transform.position.z);
		Vector3 end = new Vector3 (target.position.x, 0, target.position.z);
		float dist = Vector3.Distance (start, end);

		//transform.position = Vector3.Lerp (startPosition, target.position, Time.deltaTime * arriveSpeed);

		if (dist > distance) {
			move (arriveSpeed);
		} else {
			startMoving = false;
			StartCoroutine ("goawayCoroutine");
		}
	}

	public void showPackage(bool show){
		package.SetActive (show);
	}

	public IEnumerator gotSelected(){
		anim.SetBool ("showRadar", false);
		showSelected (true);
		yield return new WaitForSeconds (1.5f);
		showSelected (false);
		showBid (false);
		showRadar (false);
	}

	public void showSelected(bool show){
		anim.SetBool ("selected", show);
	}

	public void showRadar(bool show){
		radar.SetActive (show);
		anim.SetBool ("showRadar", show);
	}

	private void move(float speed){
		transform.position += transform.forward * Time.deltaTime * speed;
	}

	private void rise(float speed){
		if (transform.position.y - startPosition.y <= riseHeight) {
			transform.position += transform.up * Time.deltaTime * speed;
		} else {
			startRising = false;
			StartCoroutine ("moveCoroutine");
		}
	}


	public void showBid(bool show){
		if (show) {
			int rnd = Random.Range (minBid, maxBid);
			rotateBidToCamera ();
			setBidValue (rnd.ToString());
		}

		bid.SetActive (show);
		anim.SetBool ("showBid", show);
	}

	public void setBidValue(string value){
		bidValue.text = value;
	}

	void Update () {
		if (Input.GetKeyDown ("b")) {
			showBid (true);
		}

		if (Input.GetKeyDown ("n")) {
			showBid (false);
		}

		if (Input.GetKeyDown ("r")) {
			showRadar (true);
		}

		if (Input.GetKeyDown ("t")) {
			showRadar (false);
		}

		if (Input.GetKeyDown ("v")) {
			setBidValue ("30");
		}

		if (Input.GetKeyDown ("q")) {
			Debug.Log ("YAA");
			rotateToTarget = true;
		}

		if (rotateToTarget) {
			rotateTowardsTarget ();
		}

		if (startMoving) {
			moveToTarget (target);
		}

		if (goAway) {
			move (leaveSpeed);
		}

		if (startRising) {
			rise (riseSpeed);
		}
	}
}
