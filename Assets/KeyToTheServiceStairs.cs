using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyToTheServiceStairs : KeyBehaviour {	
	
	public AudioSource audioSourceToFadeIn;
	public GameObject sleepingNunToDisable;
	public GameObject nunToEnable;
	
	private bool fadeIn=false;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	public override void ExecuteAction(bool instantAction=false){
		if(!instantAction) fadeIn=true;
		else audioSourceToFadeIn.volume=0f;
		sleepingNunToDisable.SetActive(false);
		nunToEnable.SetActive(true);
		LevelState.getInstance().keyToServiceStairs=true;
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				ExecuteAction();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition()){
				ExecuteAction();
			}
		}
		if(fadeIn){
			float diff = Time.deltaTime;
			if(audioSourceToFadeIn.volume-diff>0f)
				audioSourceToFadeIn.volume-=diff*0.5f;
			else{
				audioSourceToFadeIn.volume=0f;
				fadeIn=false;
			}
		}
	}
}
