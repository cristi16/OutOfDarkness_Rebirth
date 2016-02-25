using UnityEngine;
using System.Collections;

public class CameraZoomer : MonoBehaviour {

	private float timeToZoom=0.7f;
	private float speed=0f;
	private float initialFOV;
	private float finalFOV=80f;
	private bool firstTime=true;
	private HidingController hidingController;

	// Use this for initialization
	void Start () {	
		hidingController = GameObject.FindGameObjectWithTag ("Kid").GetComponent<HidingController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (firstTime) {
			initialFOV=Camera.main.fieldOfView;
			firstTime=false;
			speed=(finalFOV-initialFOV)/timeToZoom;
		}

		if (!hidingController.hiding) {
			if (Input.GetButton ("Zoom")) {
				if(Camera.main.fieldOfView>finalFOV){
					foreach(Camera c in GetComponentsInChildren<Camera>()) c.fieldOfView+=speed*Time.deltaTime;
				}
			} else { 
				if(Camera.main.fieldOfView<initialFOV){
					foreach(Camera c in GetComponentsInChildren<Camera>()) c.fieldOfView-=speed*Time.deltaTime;
				}
			}
		}

	}

	void StartZoom(){
	}

	void StopZoom(){
	}

}
