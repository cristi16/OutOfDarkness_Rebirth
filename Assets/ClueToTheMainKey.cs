using UnityEngine;
using System.Collections;

public class ClueToTheMainKey : MonoBehaviour {
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	
	// Use this for initialization
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){				
				LevelState.getInstance().foundClueToKey=true;				
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && (interactiveObject.showingInteractiveObject)){
				LevelState.getInstance().foundClueToKey=true;						
			}		
		}		
	}
}
