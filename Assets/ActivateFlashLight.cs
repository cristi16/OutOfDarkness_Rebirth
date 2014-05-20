using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateFlashLight : MonoBehaviour {
		
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				LevelState.getInstance().flashlightActivated=true;
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition()){
				LevelState.getInstance().flashlightActivated=true;
			}
		}		
	}
}
