using UnityEngine;
using System.Collections;

public class SwitchMechanic_Script : MonoBehaviour {
	
	
	
	internal GameObject kid;
	internal GameObject ghost;
	private GameObject camera;
	
	private CheckpointsManager_Script gameManager;
	private MenuManager menuManager;
	internal bool swapActivated=true;
	
	private static bool kid_control;
	
	public float max_switchback_distance;
	
	public float ghost_y_after_switch;
	
	public bool line_of_sight = false;
	
	private float last_switch;
	
	private RaycastHit hit;
	private bool firstTime = true;
	private bool controlDisabled = false;
	
	private AudioSource audio;
	public AudioClip switchsound;
	internal float lastSwitchToKid = 0;
	
	
	// Use this for initialization
	void Start () {
		kid = GameObject.FindGameObjectWithTag("Kid");
		ghost = GameObject.Find("Ghost");
		camera = Camera.mainCamera.gameObject;
		audio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		if(kid == null || ghost == null || camera == null){
			Debug.Log("Inizialization error in switch mechanic");
			return;
		}
		
		//if(LevelState.getInstance().ghostFollowsKidFromStart == false)
		//	kid.GetComponent<LookForGhost>().enabled = false;
		
		GameObject temp = GameObject.FindGameObjectWithTag("GameController");
		gameManager = temp.GetComponent<CheckpointsManager_Script>();	
		menuManager = temp.GetComponent<MenuManager>();
		
		kid_control = true;
		SetControl(false);
		
		if(menuManager.isMainMenu || menuManager.isCredits || menuManager.isControls)
		{
			DisableControl();
		}
		
		//swapActivated = LevelState.getInstance().swapActivatedFromStart;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(menuManager.isMainMenu || menuManager.isCredits || menuManager.isControls) 
			return;
		else
		{
			if(firstTime)
			{
				EnableControl();
				firstTime = false;
			}
		}
		
		if(controlDisabled) return;
		
		//Debug.DrawRay(ghost.transform.position, kid.transform.position-ghost.transform.position, Color.red);
		if(Input.GetButtonUp("Swap") && last_switch < Time.time && swapActivated){
			
			if(kid_control){ //Kid has the control, wait for the ghost to be near
				if(line_of_sight){
					if(Physics.Raycast(ghost.transform.position, kid.transform.position-ghost.transform.position,out hit)){
						if(hit.distance <= max_switchback_distance && hit.transform.CompareTag("Kid")) 
						//if(hit.distance <= max_switchback_distance)
						{
							//Debug.Log("found");
							SetControl(true);
						}
					}
				}else{
					if(Vector3.Distance(ghost.transform.position, kid.transform.position) <= max_switchback_distance){
						//Debug.Log("found");
						SetControl(true);
					}
				}
			}else{ //Kid has the control, just switch
				lastSwitchToKid = Time.time;
				SetControl(true);
			}
			
			last_switch = Time.time + 1;
		}
	}
	
	void SetControl(bool switchControl)
	{
		audio.PlayOneShot(switchsound);
		if(switchControl) kid_control = !kid_control;
		
		if(kid_control){ //When you're controlling the kid
			
			SetKidControl();
		}else{ //You're controlling the ghost
			
			SetGhostControl();
		}
	}
	
	public void DisableControl()
	{
		controlDisabled = true;
		kid.GetComponent<TP_Controller>().hasControl = false;
		kid.GetComponentInChildren<KidAnimationController>().hasControl = false;
		ghost.GetComponent<TP_Controller>().hasControl = false;
		camera.GetComponent<TP_Camera>().ignoreInput = true;
	}
	
	public void DisableControlEndOfLevel()
	{
		controlDisabled = true;
		//kid.GetComponent<TP_Controller>().walkOnlyForward = true;
	}
	
	public void EnableControl()
	{
		controlDisabled = false;
		camera.GetComponent<TP_Camera>().ignoreInput = false;
		kid.GetComponentInChildren<KidAnimationController>().hasControl = true;
		SetControl(false);		
	}
	
	public static bool getKidControl(){
		return kid_control;
	}
	
	private void SetGhostControl()
	{
		ghost.GetComponent<CharacterController>().enabled = true;
			
		ghost.GetComponent<NavMeshAgent>().enabled = false;									
		
		kid.GetComponent<TP_Controller>().hasControl=false;
		
		Camera.mainCamera.GetComponent<ColorCorrectionCurves>().enabled=true;
		Camera.mainCamera.GetComponent<NoiseEffect>().enabled=true;
		
		KidAnimationController kidAnim = kid.GetComponentInChildren<KidAnimationController>();
		
		if(kidAnim != null)
			kidAnim.ResetToIdle();
		
		GhostAnimationController ghostAnim = ghost.GetComponentInChildren<GhostAnimationController>();
		
		if(ghostAnim != null)
			ghostAnim.hasControl = true;
		
		ghost.GetComponent<TP_Controller>().hasControl=true;		
		ghost.GetComponent<FloatingGhost_Script>().setControllingGhost(true);
		
		ghost.transform.position = new Vector3(ghost.transform.position.x,ghost_y_after_switch,ghost.transform.position.z);
		
		camera.GetComponent<TP_Camera>().targetLookAt = ghost.transform.FindChild("CameraLookAtPoint");
		
		ghost.transform.FindChild("Point light").GetComponent<Light>().enabled = true;
		
		Camera.mainCamera.cullingMask = LayerMask.NameToLayer("Everything");
		int cullingMask = 1 << LayerMask.NameToLayer("KidVision");
		Camera.mainCamera.cullingMask = ~cullingMask;
		
	}
	
	private void SetKidControl()
	{
		ghost.GetComponent<CharacterController>().enabled = false;
		
		ghost.GetComponent<NavMeshAgent>().enabled = true;
		
		kid.GetComponent<TP_Controller>().hasControl = true;
		
		KidAnimationController kidAnim = kid.GetComponentInChildren<KidAnimationController>();
				
		if(kidAnim != null)
			kidAnim.hasControl = true;
			
		GhostAnimationController ghostAnim = ghost.GetComponentInChildren<GhostAnimationController>();
		
		if(ghostAnim != null)
			ghostAnim.ResetToIdle();
		
		ghost.GetComponent<TP_Controller>().hasControl=false;
		ghost.GetComponent<FloatingGhost_Script>().setControllingGhost(false);
		
		camera.GetComponent<TP_Camera>().targetLookAt = kid.transform.FindChild("CameraLookAtPoint");
		Camera.mainCamera.GetComponent<ColorCorrectionCurves>().enabled=false;
		Camera.mainCamera.GetComponent<NoiseEffect>().enabled=false;
		ghost.transform.FindChild("Point light").GetComponent<Light>().enabled = false;
		
		Camera.mainCamera.cullingMask = LayerMask.NameToLayer("Everything");
		int cullingMask = 1 << LayerMask.NameToLayer("GhostVision");
		Camera.mainCamera.cullingMask = ~cullingMask;	
		
	}
}
