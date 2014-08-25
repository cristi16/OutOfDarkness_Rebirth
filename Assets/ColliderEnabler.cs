using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderEnabler : MonoBehaviour {
	
	public List<Collider> collidersToBeEnabled;
	public List<Collider> collidersToBeDisabled;
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	public bool enableHelpIfColliderHasHelp=true;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	public void Enable(){
		foreach(Collider c in collidersToBeEnabled){ 
			c.enabled=true;
			if(enableHelpIfColliderHasHelp && c.GetComponent<HelpTrigger>()!=null){
				c.GetComponent<HelpTrigger>().activateHelp();
			}					
		}
	}

	public void Disable(){
		foreach(Collider c in collidersToBeDisabled){ 
			c.enabled=false;							
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				Enable ();
				Disable();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && interactiveObject.showingInteractiveObject){
				Enable ();
				Disable();
			}
		}		
	}
	
	void OnTriggerEnter(Collider other){
		if(hotSpot==null && interactiveObject==null && other.tag=="Kid"){
			Enable();
			Disable();
		}
	}
}
