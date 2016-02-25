using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionOnSight : MonoBehaviour {
	
	public List<ActionOOD> actionsToExecute;
	public float delay=0f;
	private bool executed=false;	
	private bool inTrigger=false;
	public bool executeInsideTrigger=false;
	
	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {			
	}
	
	void SendNotices(){
		if(actionsToExecute!=null){
			foreach(ActionOOD a in actionsToExecute){
				a.execute();
			}
		}
	}
	
	void OnMouseOver(){	
		if(!executed && (!executeInsideTrigger || inTrigger)){			
			executed=true;
			Invoke("SendNotices",delay);
		}			
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			inTrigger=true;
			if(!executed && executeInsideTrigger){			
				executed=true;
				Invoke("SendNotices",delay);
			}	
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.tag=="Kid"){
			inTrigger=false;
		}
	}
}
