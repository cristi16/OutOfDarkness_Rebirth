using UnityEngine;
using System.Collections;

public class HelpTriggerEnabler : MonoBehaviour {
	
	public HelpTrigger help;
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	public bool activateHelpInstantly=true;
	public bool activateHelpBeforeInteraction=false;	
	
	// Use this for initialization
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		if(help==null) return;
		if(hotSpot!=null){
			if(hotSpot.getGui()){
				//help.gameObject.SetActive(true);
				if(activateHelpInstantly) help.activateHelp();
				else help.GetComponent<Collider>().enabled=true;
				Destroy (this);
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && ((interactiveObject.showingInteractiveObject) || activateHelpBeforeInteraction)){
				//help.gameObject.SetActive(true);
				if(activateHelpInstantly) help.activateHelp();
				else help.GetComponent<Collider>().enabled=true;
				Destroy (this);
			}		
		}		
	}
	
}
