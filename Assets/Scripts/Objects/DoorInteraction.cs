using UnityEngine;
using System.Collections;


public class DoorInteraction : MonoBehaviour {
	
	public enum DoorState { Idle, Opening, Closing }
	
	/// Specifies if the door is locked or not
	public bool initiallyLocked;
	/// Specifies if the door has an amulet or not
	public bool hasAmulet = false;
	/// Specifies if the door is unusable or jammed
	public bool isUnusable = false;
	
	public bool blockDoorAfterPass = false;
	/// Smooth factor for door rotation
	public float smoothFactor = 0.2f;
	/// Controls how fast the door is opening or closing
	public float maxSpeed = 150f;
	
	private AudioClip unlockSound;
	private AudioClip openDoorSound;
	private AudioClip closeDoorSound;
	private AudioClip getAmuletSound;
	private AudioClip lockedDoorSound;
	
	private bool opensOnInside = true;
	/// Specifies if we have used the key on the door
	internal bool usedKey = false;
	internal bool hasKey = false;
	internal DoorState state = DoorState.Idle;
	internal bool isClosed = true;
	private float angleOpenedInside = 180f;
	private float angleOpenedOutside = 0f;
	private float angleClosed = 90f;
	private Transform doorTransform;
	private Vector3 angles;
	private float velocity = 0f;
	private CheckpointsManager_Script gameManager;
	private AudioSource audioSource;
	private GameObject amulet;
	private bool playerInRange = false;
	private float openingSafeDistance;
	private float closingSafeDistance;
	private float safeDistanceOffset = 3.8f;
	private Transform playerTransform;
	private int count = 0;
	private bool destroyTextTriggersIfAny=true;
	private InteractiveTrigger interactiveTrigger;
	private ShowText showText;
	private GameObject doorIcon;
	
	/// <summary>
	/// Use this for initialization
	/// </summary>/ 
	void Start () {
		showText = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<ShowText>();
		doorTransform = transform.parent.FindChild("DoorModel");
		GameObject gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
		audioSource = transform.parent.GetComponent<AudioSource>();
		amulet = doorTransform.FindChild("Amulet").gameObject;
		gameManager = gameManagerObj.GetComponent<CheckpointsManager_Script>();
		
		float kidRadius = gameManager.kidRadius;
		openingSafeDistance = doorTransform.lossyScale.x + kidRadius + safeDistanceOffset;
		closingSafeDistance = doorTransform.lossyScale.z + kidRadius + safeDistanceOffset;
		
		AudioManager audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		unlockSound = audio.doorKeyUnlock;
		openDoorSound = audio.doorOpen;
		closeDoorSound = audio.doorClosed;
		getAmuletSound = audio.pickUp;
		lockedDoorSound = audio.doorLocked;
		interactiveTrigger = gameObject.GetComponent<InteractiveTrigger>();
		
		if(blockDoorAfterPass == false)
			Destroy(transform.FindChild("BlockDoorTrigger").gameObject);
		
		doorIcon = GameObject.CreatePrimitive(PrimitiveType.Plane);
		doorIcon.transform.parent = this.transform;
		doorIcon.transform.localPosition = Vector3.zero;
		doorIcon.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
		doorIcon.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		Destroy(doorIcon.collider);
		doorIcon.layer = LayerMask.NameToLayer("MapIcons");
		
		// dirty fix ( the amulet was set before the saved data was received from the persistent level state
		Invoke("SetAmulet", 0.1f);
		Invoke("SetMaterial", 0.1f);
		Invoke("DestroyLockets", 0.1f);
	}
	
	void SetMaterial()
	{
		if(LevelState.getInstance().ContainsCheckedDoor(transform.parent.gameObject.name)){
			if(initiallyLocked && !usedKey)
				doorIcon.renderer.material = gameManager.lockIcon;
			else if(hasKey && !usedKey)
				doorIcon.renderer.material = gameManager.keyIcon;
			else
				doorIcon.renderer.enabled = false;
		} else {
			if(hasKey && !usedKey)
				doorIcon.renderer.material = gameManager.keyIcon;
			else
				doorIcon.renderer.material = gameManager.unknownIcon;
		}				
	}
	
	void DestroyLockets(){
		//if(!initiallyLocked || usedKey) 
			Destroy(doorTransform.FindChild("Lockets").gameObject);	
	}
	
	void SetAmulet()
	{
		if(hasAmulet)
			amulet.SetActive(true);
		else if(isUnusable)
			amulet.renderer.enabled = false;
		else
		{
			amulet.renderer.enabled = false;
			amulet.GetComponentInChildren<BoxCollider>().isTrigger = true;	
		}	
	}
	
	// Update is called once per frame
	void Update () {
				
		
		if(Input.GetButtonUp("Interaction") && playerInRange){
			//Door has been checked
			if(LevelState.getInstance().AddCheckedDoor(transform.parent.gameObject.name)){
				SetMaterial();
			}
		}
		
		// if the player is in range and presses the interaction button we first remove the amulet
//		if(SwitchMechanic_Script.getKidControl() &&  playerInRange && hasAmulet)
//		{
//			if(Input.GetButtonDown("Interaction") && state == DoorState.Idle)
//			{
//				hasAmulet = false;
//				amulet.renderer.enabled = false;
//				amulet.GetComponentInChildren<BoxCollider>().isTrigger = true;	
//				audioSource.PlayOneShot(getAmuletSound);
//				return;
//			}
//			
//		}
		// if the door is locked return
		if(initiallyLocked && !usedKey && !hasKey && playerInRange){
			if(Input.GetButtonDown("Interaction")){
				if(!isUnusable){ 
					audioSource.PlayOneShot(lockedDoorSound);					
				}
			}
			return;
		}
		
		// if neither the kid nor the nuns are in range we close the door	
		if( count == 0 ) 
		{
			if(state==DoorState.Idle && !isClosed) audioSource.PlayOneShot(closeDoorSound,0.6f);
			state  = DoorState.Closing;			
		}
		else
		{
			// if there is someone in the range we have to find out on which side of the door they are
			if(isClosed == true && state == DoorState.Idle)
			{
				if(isFacingDoor())
					opensOnInside = false;
				else
					opensOnInside = true;	
			}
			// if only the kid is in range
			if( count == 1 && playerInRange )
			{
				// ... and the door is not in a transition
				if(state == DoorState.Idle)
				{	
					// ...if the player is in control of the kid and presses the interaction button we open or close the door
					if(Input.GetButtonDown("Interaction") && interactiveTrigger.getGui())
					{
						//If closed
						if(isClosed){							
						
							//If it's a normal door
							if( (!initiallyLocked || usedKey) && isClosed){
								audioSource.pitch=1.3f;
								audioSource.PlayOneShot(openDoorSound,0.6f);
							}
							
							// if we have the key and use it for the first time we play the unlock sound and change the material of the door
							if(initiallyLocked == true && hasKey == true && usedKey == false)
							{
								// Play unlocking door sound
								audioSource.PlayOneShot(unlockSound,0.6f);
								//Destroy(doorTransform.FindChild("Lockets").gameObject);	
								usedKey = true;
								doorIcon.renderer.enabled = false;
								//showText.ShowMessage("Unlocked the\n"+transform.parent.gameObject.name+".",0.5f,0.2f,true);
								//interactiveTrigger.removeControl=false;
								//interactiveTrigger.returnControl=true;
							}
							state = DoorState.Opening;
						} else {
							//If open							
						
							//If it's a normal door
							if( (!initiallyLocked || usedKey) && !isClosed){
								audioSource.PlayOneShot(closeDoorSound,0.6f);
							}	
							state = DoorState.Closing;
						}
												
					}
					// if the kid is the only one in range and we haven't opened the door we return
					 if(state == DoorState.Idle) return;
				}
			}
			else 
			{
				// if there is at least one nun in range we open the door
				if(state==DoorState.Idle && isClosed)
				{
					audioSource.pitch=0.4f;
					audioSource.PlayOneShot(openDoorSound,0.6f);
				}
				state = DoorState.Opening;
				
			}
		}
		
		AnimateDoor();
	}
	
	private void AnimateDoor()
	{
		switch(state)
		{
		case DoorState.Opening:
				// Smooth damping
				angles = doorTransform.localEulerAngles;
				doorTransform.localRotation = Quaternion.Euler( angles.x,
				Mathf.SmoothDampAngle(angles.y, opensOnInside ? angleOpenedInside : angleOpenedOutside, ref velocity, smoothFactor, maxSpeed), angles.z);
				
				if(opensOnInside ? (doorTransform.localEulerAngles.y >= angleOpenedInside - 1) : (doorTransform.localEulerAngles.y <= angleOpenedOutside + 1))
				{
					doorTransform.localEulerAngles = new Vector3(doorTransform.localEulerAngles.x, 
					opensOnInside ? angleOpenedInside : angleOpenedOutside, doorTransform.localEulerAngles.z);
					state = DoorState.Idle;
					isClosed = false;
				}
			break;
		case DoorState.Closing:		
				// Smooth damping
				angles = doorTransform.localEulerAngles;			
				doorTransform.localRotation = Quaternion.Euler( angles.x,
				Mathf.SmoothDampAngle(angles.y, angleClosed, ref velocity, smoothFactor, maxSpeed), angles.z);
			
				if(opensOnInside ? (doorTransform.localEulerAngles.y <= angleClosed + 1) : (doorTransform.localEulerAngles.y >= angleClosed - 1))
				{
					doorTransform.localEulerAngles = new Vector3(doorTransform.localEulerAngles.x, 
					angleClosed, doorTransform.localEulerAngles.z);
					state = DoorState.Idle;
					isClosed = true;
				}
			break;
		}
	}
	
	private bool isFacingDoor()
	{
		//Debug.DrawRay(transform.position,transform.right * 10);
		Vector3 kidVector = Vector3.Normalize(playerTransform.position - transform.position);
		float dotP = Vector3.Dot(transform.right, kidVector);
		if(dotP > 0)
			return true;
		else
			return false;
	}
	
	public Vector3 GetSafePosition()
	{
		switch(state)
		{
		case DoorState.Opening:		
			return transform.position + (opensOnInside ? -transform.right : transform.right) * openingSafeDistance;
		case DoorState.Closing:
			if(opensOnInside)
				return transform.position + transform.right * closingSafeDistance;
			else
				return transform.position - transform.right * closingSafeDistance;
		default:
			return Vector3.zero;	
		}
	}
	/// <summary>
	/// Called whenever a key for this door has been collected
	/// </summary>
	public void kidUnlock(){
		//Unlock the door if the object is a key
		hasKey = true;
		if(doorIcon!=null && gameManager!=null) doorIcon.renderer.material = gameManager.keyIcon;
		if(destroyTextTriggersIfAny){
			//transform.parent.GetComponentInChildren<TextTrigger>().enabled=false;
			transform.parent.GetComponentInChildren<InteractiveTrigger>().removeControl=false;
			Destroy (transform.parent.GetComponentInChildren<TextTrigger>());
		}
	}
	
	private void OnTriggerEnter(Collider hit){
		
		if(hit.gameObject.tag == "Kid")
		{	
			playerTransform = hit.gameObject.transform;
			playerInRange = true;
			count++;
		}
		else if(hit.gameObject.tag == "Nun")
		{		
			AI nun = hit.gameObject.GetComponent<AI>();
			/*if(nun.getInvest() || nun.getChase() || nun.getGoingBack() || nun.gameObject.GetComponent<SleepingNun>()!=null)
			{
				playerTransform = hit.gameObject.transform;
				count++;
			}*/
			playerTransform = hit.gameObject.transform;
			count++;
		}
		else if(hit.gameObject.tag == "Friend")
		{		
			playerTransform = hit.gameObject.transform;
			opensOnInside=false;
			count++;
		}
	}
	
	private void OnTriggerExit(Collider hit){
		
		if(hit.gameObject.tag == "Kid")
		{			
			playerInRange = false;
			
			if(count > 0) count--;		
		} 
		else if(hit.gameObject.tag == "Nun")
		{			
			AI nun = hit.gameObject.GetComponent<AI>();
			/*if(nun.getInvest() || nun.getChase() || nun.getGoingBack() || nun.gameObject.GetComponent<SleepingNun>()!=null)
			{				
				if(count > 0) count--;
			}*/
			if(count > 0) count--;
		}		
	}
	
	public bool getUsedKey(){
		return usedKey;
	}
}
