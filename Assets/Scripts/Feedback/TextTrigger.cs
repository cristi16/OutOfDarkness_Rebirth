using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GlobalVariables {None, NurseryRhymes,NurseryRhymeCounter}

public class TextTrigger : MonoBehaviour {
	
	public List<string> messageList;
	private ShowText showText;
	public bool firstTextIsMessage=false;
	public bool secondTextIsMessage=false;
	public float xPosition=0.5f;
	public float yPosition=0.4f;
	internal Player player = Player.Kid;
	internal bool shown=false;
	public GlobalVariables variable=GlobalVariables.None;	
	
	public bool requiresLoss=false;
	public NunStateMachine nunThatMadePlayerLose;
	
	internal bool canActivate=true;		
	public bool activateIndirectly=true;		
	
	private bool showManyTimes=false;
	private TP_Controller controller;
	private HelpManager helpManager;		
	internal bool doNotShowAgain=false;
	public bool activateSpecialAction=false;
	public bool removeControl=false;

	// Use this for initialization
	void Start () {
		showText = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<ShowText>();
		List<string> newList = new List<string>();
		foreach(string message in messageList){
			string message2 = message.Replace("\\n","\n");
			newList.Add(message2);
		}
		messageList = newList;
		canActivate = !requiresLoss || (LevelState.getInstance().HasLost(nunThatMadePlayerLose));
		controller = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
		helpManager = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<HelpManager>();
		
	}
	
	// Update is called once per frame
	void Update () {
		if(showText==null) return;
		if(showText.messageQueue.Count==0 && !showText.showing && shown){
			if(removeControl) controller.returnControl();
			helpManager.showHelp();
		}
	}
	
	public void ShowMessages(){
		if(doNotShowAgain){ 
			shown=true;
			return;
		}
		
		if(isGlobalVariableActivated(variable) && GetComponentsInChildren<TextTrigger>()[1]!=null){
			GetComponentsInChildren<TextTrigger>()[1].ShowMessages();
			shown=true;
		} else if(!activateSpecialAction){
			foreach(string message in messageList){
				if(messageList.Count>0 && message==messageList[0]){					
					if(firstTextIsMessage){
						showText.ShowMessage(message, xPosition, yPosition,true);
					} else {
						showText.ShowMessage(message, xPosition, yPosition);
					}
				} else if(messageList.Count>1 && message==messageList[1]){
					if(secondTextIsMessage){
						showText.ShowMessage(message, xPosition, yPosition,true);
					} else {
						showText.ShowMessage(message, xPosition, yPosition);
					}
				} else {
					showText.ShowMessage(message, xPosition, yPosition);
				}							
			}
			
			//Add Nursery Rhyme Message
			if(variable==GlobalVariables.NurseryRhymeCounter){
				showText.ShowMessage("I've found "+(LevelState.getInstance().rhymesFound)+" out of 4 pieces so far.", xPosition, yPosition);
			} else if(variable==GlobalVariables.NurseryRhymes){
				LevelState.getInstance().rhymesFound++;
				showText.ShowMessage("I've found "+(LevelState.getInstance().rhymesFound)+" out of 4 pieces so far.", xPosition, yPosition);
			}
			
			shown=true;				
		} else {
			//Special action
			GetComponentInChildren<Action>().execute();
			shown=true;
		}
	}
	
	void OnTriggerStay(Collider other){
		if(canActivate && !activateIndirectly && !shown && other.tag==HelpManager.getName(player)){
			ShowMessages();
			if(removeControl) controller.removeControl();
			helpManager.hideHelp();			
		}
	}

	public void ActivateTextTrigger(){
		ShowMessages();
		if(removeControl) controller.removeControl();
	}

	void OnTriggerExit(Collider other){
		if(other.tag=="Kid" && showManyTimes) shown=false;
		if(showText.showing){ 
			showText.HideText();
			helpManager.showHelp();
		}
	}
	
	bool isGlobalVariableActivated(GlobalVariables gv){
		if(gv==GlobalVariables.None) return false;
		if(gv==GlobalVariables.NurseryRhymes || gv==GlobalVariables.NurseryRhymeCounter) return LevelState.getInstance().rhymesFound>=4;
		return false;
	}
}
