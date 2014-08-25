using UnityEngine;
using System.Collections;

public class SceneLockedDoor : MonoBehaviour {
	private bool firstTime=true;
	private TP_Controller controller;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			if(firstTime){
				firstTime=false;
				controller = GameObject.FindGameObjectWithTag ("Kid").GetComponent<TP_Controller> ();
				controller.removeControl();
			}
		}
	}
}
