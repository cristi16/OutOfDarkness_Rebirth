using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HelpKeys{Sneak,Swap,Interact,Move,Flashlight,Map,Run,Quest,QuestCompleted,Hide,Notepad,None}

public enum Player{Kid}

public class HelpTrigger : MonoBehaviour {
	
	public HelpKeys helpKey;	
	
	private HelpManager feedback;
	private Player player;
	private bool feedbackShown=false;
	public bool showHelpMoreThanOnce=false;
	public bool disableTriggerAfterUse=false;
	
	public bool requiresLoss=false;
	public NunStateMachine nunThatMadePlayerLose;
	private LevelState levelState;
	private bool canActivate=true;	
	private bool activateFlashlight=false;
	private bool activateSneak=false;
	public bool activateIndependently=false;
	private TP_Controller controller;
	
	public bool removeControl=false;
	private bool feedbackShownWhenRequiresLoss=false;
	
	
	// Use this for initialization
	void Start () {
		feedback = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<HelpManager>();
		levelState = LevelState.getInstance();
		canActivate = !requiresLoss || (levelState.HasLost(nunThatMadePlayerLose));
		
		if(helpKey==HelpKeys.Flashlight) activateFlashlight=true;
		if(helpKey==HelpKeys.Sneak) activateSneak=true;
		player = Player.Kid;
		controller = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void activateHelp(){
		if(canActivate && (!feedbackShown || (!feedbackShownWhenRequiresLoss && requiresLoss)) && (helpKey!=HelpKeys.Move || !levelState.shownMoveFeedback)){
			if(!requiresLoss || levelState.HasLost(nunThatMadePlayerLose)){
				//Remove control maybe?
				feedback.activate(helpKey);
				if(!showHelpMoreThanOnce) feedbackShown=true;		
				if(disableTriggerAfterUse) collider.enabled=false;
				if(activateIndependently) feedback.showHelp(); 
				if(activateFlashlight){
					LevelState.getInstance().flashlightActivated=true;
				}
				if(activateSneak) LevelState.getInstance().sneakActivated=true;
				if(helpKey==HelpKeys.Move) levelState.shownMoveFeedback=true;
				if(requiresLoss) feedbackShownWhenRequiresLoss=true;
			}			
		}
	}
	
	void OnTriggerEnter(Collider other){ 
		if(other.tag==HelpManager.getName(player)){
			activateHelp();
		}
	}
	
	void OnTriggerStay(Collider other){
		if(other.tag==HelpManager.getName(player)){
			activateHelp();
		}		
	}
}
