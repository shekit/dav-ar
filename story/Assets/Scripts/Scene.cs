using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour {

	void Start () {
		SceneManager.Instance.startScene (gameObject.transform);
	}
}
