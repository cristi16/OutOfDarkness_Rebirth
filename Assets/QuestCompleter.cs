using UnityEngine;
using System.Collections;

public class QuestCompleter : MonoBehaviour {
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	public string questID;	
	
	private HelpTrigger questCompletedTrigger;
	
	private MapManager map;
	// Use this for initialization
	
	void Start () {
		map = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
		questCompletedTrigger = GameObject.FindGameObjectWithTag("QuestCompleted").GetComponent<HelpTrigger>();
	}
	
	void CompleteQuest(){
		if(map.ContainsQuest(questID)){
			map.CompleteQuest(questID);
			questCompletedTrigger.activateHelp();
		}		
	}
	
	// Update is called once per frame
	void Update () {		
		if(hotSpot!=null){
			if(hotSpot.getGui()){
				CompleteQuest();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() && interactiveObject.showingInteractiveObject){
				CompleteQuest();
			}		
		}		
	}
}
