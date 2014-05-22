using UnityEngine;
using System.Collections;

public class CollectibleObject_Script : MonoBehaviour {
	
	private bool gui = false;
	private bool showImage=true;
	
	public float image_width = 0.5f;
	public float image_height = 0.8f;
	
	public Texture2D image;
	private float timer;
	
	private AudioSource audioSource;
	private AudioClip sound;
	private CollectiblesManager_Script collectibles_manager;
	
	internal bool already_added = false;	
	private GameObject commands;
	private InteractiveCollider interactive;
	private TP_Controller controller;
	public bool addToCollectibleCompendium=true;
	
	private TextTrigger triggerToActivate;
	private ShowText showText;
	private HelpManager helpManager;
	private GUITexture collectiblesTexture;
	public bool showTextOnlyOnce=false;
	
	// Use this for initialization
	void Start () {
		showText = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<ShowText>();
		collectibles_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CollectiblesManager_Script>();
		AudioManager am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		if( collectibles_manager== null || am == null){
			Debug.LogError("Error in the initialization of "+this.name);
			return;
		}
		collectibles_manager.countCollectibles();
		sound = am.kidInteract;
		audioSource = GetComponent<AudioSource>();
		
		LoadCollectibleState();
		commands = GameObject.FindGameObjectWithTag("Commands");
		
		interactive=GetComponent<InteractiveCollider>();
		if(interactive==null) interactive = transform.parent.GetComponentInChildren<InteractiveCollider>();
		
		controller = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
		helpManager = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<HelpManager>();
		triggerToActivate = GetComponentInChildren<TextTrigger>();
		collectiblesTexture = collectibles_manager.collectiblesTexture;
	}
	
	void OnTriggerStay(Collider col){
		if(col.CompareTag("Kid") && interactive.mouseOver){
			if( Input.GetButton("Interaction") && timer < Time.time && !gui){
				if(!audioSource.isPlaying) audioSource.PlayOneShot(sound);
				
				gui = true;
				showImage=true;
				
				//Hide help texts
				if(commands!=null){
					commands.GetComponent<showCommands>().pause=true;
				}
				helpManager.hideHelp();
				
				interactive.showingInteractiveObject = true;
				controller.removeControl();
					
				timer = Time.time + 0.3f;
				
				if(!already_added && addToCollectibleCompendium){
					collectibles_manager.addCollectible();
					already_added = true;
					LevelState.getInstance().SaveCollectible(transform.parent.name, already_added);
				}
			}
		}
	}
	
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Kid")){
			if(interactive!=null) interactive.inRange=false;
			/*gui = false;
			if(commands!=null){
				commands.GetComponent<showCommands>().pause=false;
			}
			helpManager.showHelp();
			interactive.showingInteractiveObject = false;
			controller.returnControl(false);*/
		}
	
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			if(interactive!=null) interactive.inRange=true;
		}
	}
	
	void OnGUI(){
		if(!gui)			
			return;
		
		//GUI.Label(new Rect ((Screen.width-image_dimension)/2,(Screen.height-image_dimension)/2, image_dimension, image_dimension), image);
		float width = Screen.width*image_width/3f;
		float height = Screen.height*image_height/3f;
		
		if(!TP_Motor.oculusRift || showImage) collectibles_manager.showTexture=true;
		collectiblesTexture.transform.localScale = new Vector3(width/Screen.width,height/Screen.height,0.1f);
		collectiblesTexture.texture=image;
			
		//GUI.DrawTexture(new Rect ((Screen.width - width)/2 ,(Screen.height - height)/2, width, height), image);
		
		if(triggerToActivate!=null){
			if(Input.GetButton("Interaction") && timer < Time.time){
				if(!triggerToActivate.shown) triggerToActivate.ShowMessages();
				//gui=false;
				showImage=false;
			}
			if(showText.messageQueue.Count==0 && !showText.showing && triggerToActivate.shown){
				gui=false;
				triggerToActivate.shown=false;
				if(showTextOnlyOnce) triggerToActivate.doNotShowAgain=true;
				
				
				if(commands!=null){
					commands.GetComponent<showCommands>().pause=false;
				}
				helpManager.showHelp();
				
				interactive.showingInteractiveObject = false;
				controller.returnControl(false);						
				
				timer = Time.time +0.3f;
			}
		} else {						
			if(Input.GetButton("Interaction") && timer < Time.time){
				gui=false;
				
				if(commands!=null){
					commands.GetComponent<showCommands>().pause=false;
				}
				interactive.showingInteractiveObject = false;
				controller.returnControl(false);						
				
				timer = Time.time +0.3f;
			}
		}							
	}
	
	public bool getGui(){
		return gui;
	}
	
	public void setGui(bool temp){
		gui = temp;
	}
	
	private void LoadCollectibleState()
	{
		if(LevelState.getInstance().IsCollected(transform.parent.name) == true)
		{
			already_added = true;
			collectibles_manager.addCollectible();
		}
	}
}
