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

	private void ActivateFlashlight(){
		//if(TP_Motor.oculusRift){
			GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>().EnableDisableFlashlights();
		//}
	}

	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				LevelState.getInstance().flashlightActivated=true;
				Invoke("ActivateFlashlight",1f);
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition()){
				LevelState.getInstance().flashlightActivated=true;
				Invoke("ActivateFlashlight",1f);
			}
		}		
	}
}
