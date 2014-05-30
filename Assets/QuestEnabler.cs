using UnityEngine;
using System.Collections;

public class QuestEnabler : MonoBehaviour {
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	public string questID;
	public string questDescription;
	
	private HelpTrigger questTrigger;
	
	private MapManager map;
	// Use this for initialization
	
	void Start () {
		map = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
		questTrigger = GameObject.FindGameObjectWithTag("NewQuest").GetComponent<HelpTrigger>();
	}
	
	void NewQuest(){
		if(!map.ContainsQuest(questID) && !map.ContainsCompletedQuest(questID)){
			map.AddQuest(questID,questDescription);		
			questTrigger.activateHelp();
			if(GetComponentInChildren<KidAnimationController>()!=null)
			GetComponentInChildren<KidAnimationController>().Walk();
		}		
	}
	
	// Update is called once per frame
	void Update () {		
		if(hotSpot!=null){
			if(hotSpot.getGui()){
				NewQuest();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && interactiveObject.showingInteractiveObject){
				NewQuest();
			}		
		}		
	}
}
