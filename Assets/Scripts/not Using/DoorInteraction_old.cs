using UnityEngine;
using System.Collections;


public class DoorInteraction_old : MonoBehaviour {
	
	public enum DoorState { Idle, Opening, Closing }
	
	private bool opensOnInside = true;
	/// Specifies if the door is locked or not
	public bool isLocked = false;
	/// Specifies if the door has an amulet or not
	public bool hasAmulet = false;
	/// Specifies if the door is unusable or jammed
	public bool isUnusable = false;
		/// Smooth factor for door rotation
	public float smoothFactor = 0.2f;
	/// Controls how fast the door is opening or closing
	public float maxSpeed = 150f;
	
	private AudioClip unlockSound;
	private AudioClip openDoorSound;
	//public float speedOpen;
	//public float speedClose;
	
	/// Specifies if we have used the key on the door
	private bool usedKey = false;
	private float angleOpenedInside = 180f;
	private float angleOpenedOutside = 0f;
	private float angleClosed = 90f;
	private Transform doorTransform;
	internal DoorState state = DoorState.Idle;
	internal bool isClosed = true;
	private Vector3 angles;
	private float velocity = 0f;
	private CheckpointsManager_Script gameManager;
	private AudioSource audioSource;
	private GameObject amulet;
	private bool initialState;
	private bool nunPassing = false;
	internal bool playerInRange = false;
	internal bool nunInRange = false;
	private float openingSafeDistance;
	private float closingSafeDistance;
	private float safeDistanceOffset = 3.8f;
	internal Transform playerTransform;
	public int count = 0;
	
	
	/// <summary>
	/// Use this for initialization
	/// </summary>/ 
	void Start () {
		
		doorTransform = transform.parent.FindChild("DoorModel");
		GameObject gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
		audioSource = transform.parent.GetComponent<AudioSource>();
		amulet = doorTransform.FindChild("Amulet").gameObject;
		gameManager = gameManagerObj.GetComponent<CheckpointsManager_Script>();
		
		float kidRadius = gameManager.kidRadius;
		openingSafeDistance = doorTransform.lossyScale.x + kidRadius + safeDistanceOffset;
		closingSafeDistance = doorTransform.lossyScale.z + kidRadius + safeDistanceOffset;
		
		if(isUnusable)
			doorTransform.gameObject.GetComponent<Renderer>().material = gameManager.unusableDoorMaterial;
		else if(isLocked)
			doorTransform.gameObject.GetComponent<Renderer>().material = gameManager.lockedDoorMaterial;
		else
			doorTransform.gameObject.GetComponent<Renderer>().material = gameManager.unlockedDoorMaterial;
		
		AudioManager audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		unlockSound = audio.doorKeyUnlock;
		openDoorSound = audio.doorOpen;
		
		if(hasAmulet)
			amulet.SetActive(true);
		else
			amulet.SetActive(false);
		initialState = isLocked;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.DrawLine(transform.position, transform.position + transform.right * closingSafeDistance);
		
		// if we are controlling the ghost we don't need to open doors
		if(!SwitchMechanic_Script.getKidControl() && state == DoorState.Idle) //Modified by Mattia
		{
			if(hasAmulet == false)
				doorTransform.gameObject.GetComponent<Collider>().enabled = false;
			if(!nunPassing)
				return;
		}
		else
			doorTransform.gameObject.GetComponent<Collider>().enabled = true;
		
		// if the door is locked return
		if(isLocked) return;
		
		if(playerInRange || nunInRange)
		{
			if(hasAmulet && Input.GetButtonDown("Interaction") && state == DoorState.Idle)
			{
					hasAmulet = false;
					amulet.SetActive(false);
					return;
			}
				
			if( (Input.GetButtonDown("Interaction") && state == DoorState.Idle) || nunPassing)
			{					
				
				if(isClosed == true)
					if(isFacingDoor())
					opensOnInside = true;
				else
					opensOnInside = false;
				
				if(isClosed)
					state = DoorState.Opening;
				else
				{
					state = DoorState.Closing;
				}
				
				if(initialState == true && isLocked == false && usedKey == false)
				{
					// Play unlocking door sound
					audioSource.PlayOneShot(unlockSound);
					doorTransform.gameObject.GetComponent<Renderer>().material = gameManager.unlockedDoorMaterial;
					usedKey = true;
				}
				
				nunPassing = false;
				playerInRange = false;
				nunInRange=false;
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
		isLocked = false;
	}
	
	public void NunPassing(bool openDoor)
	{
		if((openDoor && isClosed) || (!openDoor && !isClosed))
		{
		 	nunPassing = true;
			nunInRange = true;
		}
	}
	
	private void OnTriggerEnter(Collider hit){
		
		if(hit.gameObject.tag == "Kid")
		{	
			playerTransform = hit.gameObject.transform;
			playerInRange = true;
			count++;
		}
	}
	
	private void OnTriggerExit(Collider hit){
		
		if(hit.gameObject.tag == "Kid")
		{			
			playerInRange = false;
			
			if(state==DoorState.Idle){
				state=DoorState.Closing;
			}
			if(count > 0) count--;
			
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
}
