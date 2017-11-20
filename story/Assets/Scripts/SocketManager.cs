using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class SocketManager : Singleton<SocketManager> {

	public GameObject socketIO;
	private SocketIOComponent socket;

	// Use this for initialization
	void Start () {
		socket = socketIO.GetComponent<SocketIOComponent> ();

		socket.On ("connected", connected);
		socket.On ("order-drone", orderDrone);
		socket.On ("order-car", orderCar);
	}

	void connected(SocketIOEvent e){
		Debug.Log ("CONNECTED");
	}

	void orderDrone(SocketIOEvent e){
		SceneManager.Instance.createMarker ("drone");
	}

	void orderCar(SocketIOEvent e){
		SceneManager.Instance.createMarker ("car");
	}

}
