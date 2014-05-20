using UnityEngine;
using System.Collections;

public class FoundRhyme : MonoBehaviour {

	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	private bool activated=false;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(hotSpot!=null && !activated){
			if(hotSpot.getGui()){	
				activated=true;
				LevelState.getInstance().rhymesFound++;
			}
		}
		
		if(interactiveObject!=null && !activated){
			if(interactiveObject.activateHelpCondition()){
				activated=true;
				LevelState.getInstance().rhymesFound++;
			}
		}		
	}
}
