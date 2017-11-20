using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPanel : MonoBehaviour {

	public Text status;

	public void setStatus(string s){
		status.text = s;
	}
}
