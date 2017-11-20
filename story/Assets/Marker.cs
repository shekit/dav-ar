using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour {

	public GameObject package;
	public GameObject point;
	// Use this for initialization

	public void showPackage(bool show){
		Debug.Log ("show package message");
		package.SetActive (show);
	}

	public void showPoint(bool show){
		point.SetActive (show);
	}
}
