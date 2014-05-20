using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderEnabler : MonoBehaviour {
	
	public List<Collider> collidersToBeEnabled;
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	public void Enable(){
		foreach(Collider c in collidersToBeEnabled){ 
			c.enabled=true;
			if(c.GetComponent<HelpTrigger>()!=null){
				c.GetComponent<HelpTrigger>().activateHelp();
			}					
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				Enable ();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && interactiveObject.showingInteractiveObject){
				Enable ();
			}
		}		
	}
	
	void OnTriggerEnter(Collider other){
		if(hotSpot==null && interactiveObject==null && other.tag=="Kid"){
			Enable();
		}
	}
}
