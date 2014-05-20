using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundFader : MonoBehaviour {	
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	
	public AudioSource audioSourceToFadeIn;
	private bool fadeIn=false;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				fadeIn=true;
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition()){
				fadeIn=true;
			}
		}
		if(fadeIn){
			float diff = Time.deltaTime;
			if(audioSourceToFadeIn.volume-diff>0f)
				audioSourceToFadeIn.volume-=diff*0.5f;
		}
	}
}
