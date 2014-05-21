using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HelpManager : MonoBehaviour {
	
	public FeedbackKey sneakHelp;
	public FeedbackKey swapHelp;
	public FeedbackKey interactHelp;
	public FeedbackKey moveHelp;
	public FeedbackKey flashlightHelp;
	public FeedbackKey mapHelp;
	public FeedbackKey runHelp;
	public FeedbackKey questNotice;
	public FeedbackKey questCompletedNotice;
	public FeedbackKey hideNotice;
		
	public Queue<HelpKeys> keyQueue;
	internal List<HelpKeys> feedbackShown;	
	public bool repeatFeedback=false;
	
	private FeedbackKey activeKey;
	
	void OnDestroy(){
		LevelState.getInstance().feedbackShown=feedbackShown;
	}	
	
	public void showHelp(){
		foreach(GUITexture texture in GetComponentsInChildren<GUITexture>()){
			texture.enabled=true;
		}
	}
	
	public void hideHelp(){
		foreach(GUITexture texture in GetComponentsInChildren<GUITexture>()){
			texture.enabled=false;
		}		
	}
	
	public static string getButtonName(HelpKeys key){
		switch(key){
			case HelpKeys.Interact:
				return "Interaction";
				break;
			case HelpKeys.Sneak:
				return "Sneak";
				break;
			case HelpKeys.Swap:
				return "Swap";
				break;		
			case HelpKeys.Flashlight:
				return "Light";
				break;
			case HelpKeys.Map:
				return "Map";
				break;
			case HelpKeys.Run:
				return "Run";
				break;
			case HelpKeys.Quest:
				return "Quest";
				break;
			case HelpKeys.QuestCompleted:
				return "QuestCompleted";
				break;
			case HelpKeys.Hide:
				return "Hide";
				break;
		}
		return null;
	}
	
	private bool anyActiveKey(){
		if(sneakHelp.active || swapHelp.active || interactHelp.active || moveHelp.active || flashlightHelp.active || mapHelp.active) return true;
		return false;
	}
	
	public void activate(HelpKeys key){
		keyQueue.Enqueue(key);			
	}
	
	private void ShowKey(){
		if(!anyActiveKey()){
			if(keyQueue.Count>0){
				HelpKeys key = keyQueue.Dequeue();
				if(repeatFeedback || !feedbackShown.Contains(key)){
					switch(key){
						case HelpKeys.Interact:
							interactHelp.active=true;
							activeKey=interactHelp;
							break;
						case HelpKeys.Sneak:
							sneakHelp.active=true;
							activeKey=sneakHelp;
							break;
						case HelpKeys.Swap:
							swapHelp.active=true;
							activeKey=swapHelp;
							break;			
						case HelpKeys.Move:
							moveHelp.active=true;
							activeKey=moveHelp;
							break;		
						case HelpKeys.Flashlight:
							flashlightHelp.active=true;
							activeKey=flashlightHelp;
							if(TP_Motor.oculusRift){
								GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>().EnableDisableFlashlights();
							}
							break;	
						case HelpKeys.Map:
							mapHelp.active=true;
							activeKey=mapHelp;
							break;
						case HelpKeys.Run:
							runHelp.active=true;
							activeKey=runHelp;
							break;
						case HelpKeys.Quest:
							questNotice.active=true;
							activeKey=questNotice;
							break;
						case HelpKeys.QuestCompleted:
							questCompletedNotice.active=true;
							activeKey=questCompletedNotice;
							break;
						case HelpKeys.Hide:
							hideNotice.active=true;
							activeKey=hideNotice;
							break;
					}
					feedbackShown.Add(key);
				}				
			}			
		}
	}
	
	FeedbackKey GetActiveKey(){
		if(anyActiveKey()) return activeKey;
		return null;
	}
	
	// Use this for initialization
	void Start () {		
		keyQueue = new Queue<HelpKeys>();
		feedbackShown = LevelState.getInstance().feedbackShown;			
	}
	
	// Update is called once per frame
	void Update () {
		ShowKey();
		if(keyQueue.Count>0){
			if(GetActiveKey()!=null) GetActiveKey().removeIt=true;
		}
	}	
	
	public static string getName(Player player){
		string name = "";
		switch(player){
			case Player.Kid:
				name = "Kid";
				break;
		}
		return name;
	}
}
