using UnityEngine;
using System.Collections;

public class TP_Controller : MonoBehaviour {

	internal CharacterController characterController;
	private TP_Motor motor;
	public bool hasControl = true;
	internal bool walkOnlyForward = false;
	private AudioSource audio;	
	private AudioManager audioManager;
	private MapManager mapManager;
	private HelpManager helpManager;
	public bool secondAnalogUpAndDown = false;

	private GameObject[] flashlights;

	public float XRotation = 0.0f;
	public float YRotation = 0.0f;
	public float rotationCoef = 10f;
	
	private OVRCameraController CameraController;

	
	public enum Direction
	{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward, Sneaking
	}
	
	public Direction MoveDirection { get; set; }
	
	public float deadZone = 0.1f;
	internal SneakWalkRunController player_sneak;
	
	void Awake()
	{
		player_sneak = gameObject.GetComponent<SneakWalkRunController>();
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
		// get the reference of the audio source only if this script is on the kid gameobject
		if( player_sneak != null)
			audio = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
		
		characterController=GetComponent("CharacterController") as CharacterController;
		motor = gameObject.GetComponent<TP_Motor>();
		//instance=this;
		//TP_Camera.UseExistingOrCreateNewMainCamera();
		
		flashlights = GameObject.FindGameObjectsWithTag("Flashlight");	
		
		helpManager = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<HelpManager>();

		CameraController = gameObject.GetComponentInChildren<OVRCameraController>();
	}
	
	public void removeControl(){
		hasControl=false;
		foreach(MouseLook ml in GetComponentsInChildren<MouseLook>()){
			ml.enabled=false;
		}
		foreach(Headbobber hb in GetComponentsInChildren<Headbobber>()) hb.enabled=false;
	}
	
	public void restoreNormalRotation(float timeToChange){
		foreach(MouseLook ml in GetComponentsInChildren<MouseLook>()){
			ml.restoreNormalRotation(timeToChange);
		}
	}
	
	public void returnControl(bool resetCamera=true){
		hasControl=true;
		foreach(MouseLook ml in GetComponentsInChildren<MouseLook>()){
			ml.enabled=true;
			if(resetCamera) ml.Start();
		}
		foreach(Headbobber hb in GetComponentsInChildren<Headbobber>()) hb.enabled=true;
	}
	
	void ActivateMap(){
		removeControl();
		helpManager.hideHelp();
		mapManager.ShowMap();
		audio.clip = audioManager.kidInteract;
		audio.pitch=1.0f;
		audio.PlayOneShot(audio.clip);
	}
	
	void DeactivateMap(){	
		returnControl(false);
		helpManager.showHelp();
		mapManager.HideMap();
		audio.clip = audioManager.kidInteract;
		audio.pitch=1.0f;
		audio.PlayOneShot(audio.clip);
	}

	public void EnableDisableFlashlights(bool enable){

		if (flashlights [0].GetComponent<Light>().enabled != enable && (!enable || player_sneak.canGetUp)) {
			foreach(GameObject flashlight in flashlights){
				//if(TP_Motor.oculusRift) flashlight.light.enabled=true;
				//else 

				/*if(flashlight.GetComponent<AudioSource>()!=null){
					if(enable){
						flashlight.GetComponent<LanternLight>().NormalMidangle();
					} else {
						flashlight.GetComponent<LanternLight>().LowMidangle();
					}
				}*/
				flashlight.GetComponent<Light>().enabled=enable;

			}

			FlashlightSound(enable);
		}

	}

	public void EnableDisableFlashlights(){
		foreach(GameObject flashlight in flashlights){
			//if(TP_Motor.oculusRift) flashlight.light.enabled=true;
			//else 
				flashlight.GetComponent<Light>().enabled=!flashlight.GetComponent<Light>().enabled;
				

			if(flashlight.GetComponent<Light>().enabled){
				FlashlightSound(true);
			} else {				
				FlashlightSound(false);
			}
		}
	}
	
	void Update() 
	{
//		if(Input.GetKeyDown(KeyCode.C))
//		{
//			if(hasControl)
//				removeControl();
//			else returnControl(false);
//		}
		
		if(player_sneak != null)
		{
			PlayLocomotionSounds();
		}				
		
		/*if(Input.GetKeyDown(KeyCode.M) && LevelState.getInstance().mapActivated){ 
			if(!mapManager.showingMap){
				if(hasControl){
					ActivateMap();
				}
			} else {
				DeactivateMap();
			}
			
		}*/				
		
		if(mapManager.showingMap && Input.GetButtonDown("Interaction")){
			DeactivateMap();
		}
		
		if(Input.GetKeyDown(KeyCode.Escape) && mapManager.showingMap)
		{
			DeactivateMap();
		}
		
		if(Input.GetKeyDown(KeyCode.F) && LevelState.getInstance().flashlightActivated && !mapManager.showingMap){ 
			EnableDisableFlashlights();
		}				

		if (TP_Motor.oculusRift) {
			RotateOculus ();
		}

		if(hasControl == false)
			return;
		
		if(Camera.main ==null)
			return;
		
		GetLocomotionInput();				
		
		motor.UpdateMotor();
	}

	void RotateOculus(){
		// compute for key rotation
		float rotateInfluence = Time.deltaTime * 60f * 1.5f;
		
		if (Input.GetKey(KeyCode.Q)) 
			YRotation -= rotateInfluence * rotationCoef;  
		if (Input.GetKey(KeyCode.E)) 
			YRotation += rotateInfluence * rotationCoef; 
		
		float hr = Input.GetAxis ("HorizontalRotation");
		
		if (hr<-0.3f) 
			YRotation += rotateInfluence * rotationCoef * hr;  
		if (hr>0.3f) 
			YRotation += rotateInfluence * rotationCoef * hr;  
		
		YRotation = YRotation % 360;
		if (YRotation < 0) YRotation += 360;
		CameraController.SetYRotation(YRotation+motor.YOffset);


		if (Input.GetKey(KeyCode.I)) 
			XRotation -= rotateInfluence * rotationCoef;  
		if (Input.GetKey(KeyCode.K)) 
			XRotation += rotateInfluence * rotationCoef; 

		float vr = Input.GetAxis ("VerticalRotation");
		
		if (vr<-0.3f) 
			XRotation += rotateInfluence * rotationCoef * vr;  
		if (vr>0.3f) 
			XRotation += rotateInfluence * rotationCoef * vr;  
		
		//comment here to remove second analog going up and down
		if (secondAnalogUpAndDown) {
			CameraController.SetXRotation (XRotation);
		}

		}

	void GetLocomotionInput()
	{		


		motor.moveVector = Vector3.zero;
		float verticalInput = Input.GetAxis("Vertical");
		float horizontalInput = Input.GetAxis("Horizontal");
		if(verticalInput > deadZone || verticalInput < -deadZone)
		{
			if(player_sneak != null && walkOnlyForward && verticalInput < 0) // disable walking backwards
				verticalInput = 0;
			
			motor.moveVector += new Vector3(0, 0, verticalInput);		
		}
		
		if(horizontalInput > deadZone || horizontalInput < -deadZone)
		{
			if(player_sneak != null && walkOnlyForward)
				horizontalInput = 0;
			
			motor.moveVector += new Vector3(horizontalInput, 0, 0);
		}
		
		DetermineCurrentMoveDirection();
	}
	
	public void DetermineCurrentMoveDirection()
	{
		
		bool forward = false;
		bool backward = false;
		bool left = false;
		bool right = false;
		
		if (motor.moveVector.z > 0)
			forward = true;
		if (motor.moveVector.z < 0)
			backward = true;
		if (motor.moveVector.x > 0)
			right = true;
		if (motor.moveVector.x < 0)
			left = true;
		
		if (forward)
		{
			if (left)
				MoveDirection = Direction.LeftForward;
			else if (right)
				MoveDirection = Direction.RightForward;
			else
				MoveDirection = Direction.Forward;
		}
		else if (backward)
		{
			if (left)
				MoveDirection = Direction.LeftBackward;
			else if (right)
				MoveDirection = Direction.RightBackward;
			else
				MoveDirection = Direction.Backward;
		}
		else if (left)
			MoveDirection = Direction.Left;
		else if (right)
			MoveDirection = Direction.Right;
		else
			MoveDirection = Direction.Stationary;		
	}
	
	void FlashlightSound(bool activate){
		if(activate){
			if(flashlights[0].GetComponent<AudioSource>()!=null) flashlights[0].GetComponent<AudioSource>().Play();
			else flashlights[1].GetComponent<AudioSource>().Play();
		} else {
			if(flashlights[0].GetComponent<AudioSource>()!=null) flashlights[0].GetComponent<AudioSource>().Play();
			else flashlights[1].GetComponent<AudioSource>().Play();
		}
	}
	
	private void PlayLocomotionSounds()
	{
		if ( motor.moveVector != Vector3.zero && hasControl)
		{			
			if(player_sneak.getRun()){
				audio.clip = audioManager.kidRunning;			
				audio.pitch=1.55f;
			} else if(player_sneak.getSneak()){
				audio.clip = audioManager.kidSneaking;
				audio.pitch=1.0f;
			} else {
				audio.clip = audioManager.kidWalking;
				audio.pitch=1.1f;
			}						
			if (audio.isPlaying == false && audio!=null) 
				audio.Play();
		}
		else
		{
			if (audio!=null)
				audio.Pause();
		}	
	}
	
}
