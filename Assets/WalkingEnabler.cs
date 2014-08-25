using UnityEngine;
using System.Collections;

public class WalkingEnabler : MonoBehaviour {
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	private bool firstTime = true;
	
	private MapManager map;
	// Use this for initialization
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	void ActivateWalking(){
		if(firstTime){
			firstTime=false;
			if(GetComponentInChildren<KidAnimationController>()!=null)
				GetComponentInChildren<KidAnimationController>().Walk();
		}		
	}
	
	// Update is called once per frame
	void Update () {		
		if(hotSpot!=null){
			if(hotSpot.getGui()){
				ActivateWalking();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && interactiveObject.showingInteractiveObject){
				ActivateWalking();
			}		
		}		
	}
}
