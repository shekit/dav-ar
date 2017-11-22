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

	public void setScale(){
		float val = PlaceMessage.Instance.getSceneSize ();
		Vector3 local = gameObject.transform.localScale;
		gameObject.transform.localScale = new Vector3 (local.x, local.y, local.z) * val;
	}
}
