using UnityEngine;
using System.Collections;

public class InteractiveTrigger : MonoBehaviour {
	
	private bool gui = false;
		
	private float timer;
	
	private AudioSource audioSource;	
		
	private InteractiveCollider interactive;
	private TP_Controller controller;	
	
	private TextTrigger triggerToActivate;
	private ShowText showText;
	private HelpManager helpManager;
	public AudioClip audioClip;	
	public bool removeControl=true;
	public bool disableAfterUse=false;
	public bool destroyAfterUse=false;
	public bool activateKey=false;
	public bool activateTextTrigger=true;
	
	internal bool returnControl=false;
	private bool hasInteractiveCollider=true;
	
	private Key_Script key;
	
	// Use this for initialization
	void Start () {
		showText = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<ShowText>();		
		AudioManager am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
				
		audioSource = GetComponent<AudioSource>();
				
		interactive = GetComponentInChildren<InteractiveCollider>();
		if(interactive==null && transform.parent!=null){ 
			interactive = transform.parent.GetComponentInChildren<InteractiveCollider>();
			if(interactive==null && transform.parent!=null){ 
				interactive = transform.parent.GetComponent<InteractiveCollider>();
			}
		}
		if(interactive==null) hasInteractiveCollider=false;
		
		controller = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
		helpManager = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<HelpManager>();
		triggerToActivate = GetComponentInChildren<TextTrigger>();		
		if((triggerToActivate==null || !triggerToActivate.enabled) && transform.parent!=null){
			triggerToActivate = transform.parent.GetComponentInChildren<TextTrigger>();		
		}
		if(activateKey){
			key=GetComponent<Key_Script>();
		}
	}
	
	void OnTriggerStay(Collider col){
		if(col.tag=="Kid"){
			if(interactive!=null) interactive.inRange=true;
		}
		if(col.CompareTag("Kid") && ((hasInteractiveCollider && interactive.mouseOver) || (!hasInteractiveCollider && triggerToActivate.canActivate))){
			if((!hasInteractiveCollider || (Input.GetButton("Interaction"))) && timer < Time.time && !gui){
				if(audioSource!=null && !audioSource.isPlaying && audioClip!=null) audioSource.PlayOneShot(audioClip);
				
				gui = true;
				
				//Activate key if it exists
				if(activateKey && key!=null){
					key.gameObject.GetComponentInChildren<MeshRenderer>().enabled=false;
					key.ActivateKey();
					key=null;
				}
					
				//Hide help texts
				if(GetComponent<DoorInteraction>()==null){
					helpManager.hideHelp();
				}				
				
				if(removeControl){
					if(hasInteractiveCollider) interactive.showingInteractiveObject = true;
					controller.removeControl();
				}
					
				timer = Time.time + 0.3f;								
			}
		}
	}		
	
	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			if(interactive!=null) interactive.inRange=true;
		}
	}
	
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Kid")){
			if(interactive!=null) interactive.inRange=false;			
		}	
	}
	
	void OnGUI(){
		if(!gui)
			return;						
		
		if(activateTextTrigger && triggerToActivate!=null && triggerToActivate.enabled){
			if(!triggerToActivate.shown) triggerToActivate.ShowMessages();
			if(showText.messageQueue.Count==0 && !showText.showing && triggerToActivate.shown){
				gui=false;
				triggerToActivate.shown=false;

				helpManager.showHelp();
				
				if(removeControl){
					if(hasInteractiveCollider) interactive.showingInteractiveObject = false;
					controller.returnControl(false);						
				}
				
				if(returnControl){
					if(hasInteractiveCollider) interactive.showingInteractiveObject = false;
					controller.returnControl(false);						
					returnControl=false;
				}
				
				timer = Time.time +0.3f;
				
				if(disableAfterUse && !destroyAfterUse){
					if(hasInteractiveCollider) Destroy (interactive);
					Destroy (this);
				}
				if(disableAfterUse && destroyAfterUse){
					if(hasInteractiveCollider) Destroy (interactive);					
					collider.enabled=false;
					Destroy (transform.parent.gameObject,5f);					
				}
			}
		}							
	}
	
	public bool getGui(){
		return gui;
	}
	
	public void setGui(bool temp){
		gui = temp;
	}
	
}
