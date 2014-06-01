using UnityEngine;
using System.Collections;

public class HidingSpot : MonoBehaviour {

	private HidingController hidingController;
	private bool canUse=true;
	
	// Use this for initialization
	void Start () {
		hidingController = GameObject.FindGameObjectWithTag("Kid").GetComponent<HidingController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void DisableHidingSpot(){
		CancelInvoke("EnableHidingSpot");
		GetComponent<InteractiveTrigger>().enabled=false;
		canUse=false;
		Invoke("EnableHidingSpot",15.0f);
	}
	
	public void EnableHidingSpot(){
		canUse=true;		
		GetComponent<InteractiveTrigger>().enabled=true;
	}
	
	void OnTriggerStay(Collider other){
	}
}
