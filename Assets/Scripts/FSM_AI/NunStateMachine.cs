using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LineOfSight { NotInRange, InvestigationRange, ChaseRange }

public class NunStateMachine : StateMachineBase {
	
	public enum NunStates
	{
		Default = 0,
		Investigating = 1,
		ChasingKid = 2,
		CaughtKid = 3,
		Stuned = 4,
		SawKidHiding = 5,
		MissedKidHiding = 6,
		GetsKidOut = 7
	}
	public enum SleepingStates 
	{
		Sleeping = 8,
		Caughing = 9
	}
	
	protected AudioManager audioManager;
	private AudioSource audioSource;
	private AudioSource chaseMusic;
	private GameObject kid;
	private HidingController hidingController;
	protected NavMeshAgent agent;
	private CheckpointsManager_Script gameManager;
	
	public float viewAngle = 30f;
	public float chaseRange = 20f;
	public float investigationRange = 40f;
	protected int currentNode = 0;
	public float patrolSpeed = 3.5f;
	public float chaseSpeed = 5f;
	public float maxRotation = 20f;
	public float stopDistanceOnCatch = 2f;
	public float defaultStoppingDistance = 2f;
	private bool rotatePositive = true;
	protected Quaternion relativeForwardRotation;
	protected bool lookingAround = false;
	public float investigationTime = 10f; // Time to wait at investigation point
	protected Vector3 investigatingLocation;
	private NunChaseInvestigateManager nunManager;
	private Transform cameraTransform;
	private Quaternion lookRotation;
	
	public float mashingLength = 3f;
	private float mashTime;
	private float mashRatio;
	private float lastButtonPress = 0f;
	private GameObject nunModel;
	public float timeToWaitAfterStun = 6f;
	private bool displayKey = false;
	private List<Vector3> hidingSpots;
	protected bool isWaiting = false;
	
	//Debug Variables
	public Color investColor = Color.white;
	public Color chaseColor = Color.green;
	
	private AudioSource nunScream;
	private AudioSource audioFeedback;	
	
	private MotionBlur[] cameraMotionBlur;	
	private Light redLight;
	private bool willDeactivateChaseElements=false;
	
	protected override void OnAwake()
	{	
		//GET COMPONENTS
		agent = GetComponent<NavMeshAgent>();
		kid = GameObject.FindGameObjectWithTag("Kid");
		hidingController = kid.GetComponent<HidingController>();
		nunManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<NunChaseInvestigateManager>();
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		audioSource = gameObject.GetComponent<AudioSource>();
		cameraTransform = Camera.mainCamera.transform;
		cameraMotionBlur = cameraTransform.root.GetComponentsInChildren<MotionBlur>();
		nunModel = gameObject.GetComponentInChildren<Animator>().gameObject;
		GameObject game_manager = GameObject.FindGameObjectWithTag("GameController");
		gameManager = game_manager.GetComponent<CheckpointsManager_Script>();
		agent.stoppingDistance = defaultStoppingDistance;
		agent.speed = patrolSpeed;
		
		nunScream = GameObject.FindGameObjectWithTag("Nun Scream").GetComponent<AudioSource>();
		audioFeedback = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		chaseMusic = GameObject.FindGameObjectWithTag("ChaseMusic").GetComponent<AudioSource>();				
		redLight = GetComponentInChildren<RedLight>()==null?null:GetComponentInChildren<RedLight>().light;
		
		currentState = NunStates.Default;
	}		
	
	
	// Debugging FoV
	void LateUpdate()
	{	
		Debug.DrawLine(transform.position, transform.position + transform.forward * investigationRange, investColor);
		Quaternion rotation = Quaternion.Euler(new Vector3(0, viewAngle, 0));
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward *  investigationRange, investColor);
		rotation = Quaternion.Euler(new Vector3(0, -viewAngle, 0));
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * investigationRange, investColor);
		
		Debug.DrawLine(transform.position, transform.position + transform.forward * chaseRange, chaseColor);
		rotation = Quaternion.Euler(new Vector3(0, viewAngle, 0));	
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward *  chaseRange, chaseColor);
		rotation = Quaternion.Euler(new Vector3(0, -viewAngle, 0));
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * chaseRange, chaseColor);
	}
	
	#region Default
	
	protected void checkIfChasingNun(){
		if(NunAlertManager.getInstance().nunsChasing.Contains(this)) deactivateChaseElements();
	}
	
	protected virtual IEnumerator Default_EnterState()
	{		
		Debug.Log ("Abandon Chase Investigate");
		checkIfChasingNun (); 
		yield return null;
	}
	
	protected virtual void Default_Update()
	{
		CheckIfKidInRange();
	}
	
	protected virtual IEnumerator Default_ExitState()
	{
		yield return null;
	}
	#endregion
	
	#region Investigating
	
	protected IEnumerator Investigating_EnterState()
	{
		agent.speed = patrolSpeed;
		agent.destination = investigatingLocation;
		lookingAround = false;
		StopCoroutine("GoToState");		
		
		if(!chaseMusic.isPlaying) audioFeedback.PlayOneShot(audioManager.nunSecondAlert);
		
		NunAlertManager.getInstance().AddNun(this,false);
		//In case the nun goes was going out of a chase
		CancelInvoke("deactivateInvestigationElements");
		CancelInvoke("destroyNunAfterChase");				
		
		yield return null;
	}
	
	protected void Investigating_Update()
	{
		if(CheckIfKidInRange() == true)
		{
			StopCoroutine("GoToState");
			return; // changes state
		}
		Vector2 nunPosition = new Vector2(transform.position.x, transform.position.z);
		Vector2 investPosition = new Vector2(agent.destination.x, agent.destination.z);
		
		if (Vector2.Distance (nunPosition, investPosition) <= agent.stoppingDistance + 0.1f) { //distraction reached 	
			if (!lookingAround) {
				relativeForwardRotation = transform.rotation;
				StartCoroutine (GoToState (investigationTime, NunStates.Default));
				lookingAround = true;
			} else {
				LookAround ();		
			}
		} else {
			PlayWalkingAudioClip ();
		}
	}
	#endregion
	
	#region ChasingKid
	
	protected void activateInvestigationElements(){
		CancelInvoke("deactivateInvestigationElements");
		CancelInvoke("destroyNunAfterChase");
		
		NunAlertManager.getInstance().AddNun(this,false);
	}
	
	protected void deactivateInvestigationElements(){
		NunAlertManager.getInstance().RemoveNun(this, false);							
		
	}
	
	protected void activateChaseElements(){
		CancelInvoke("deactivateChaseElements");
		CancelInvoke("destroyNunAfterChase");
		chaseMusic.volume = chaseMusic.GetComponent<MusicManager>().audioVolume;
		chaseMusic.Play();
		if (cameraMotionBlur != null) {
			foreach(MotionBlur b in cameraMotionBlur){
				b.enabled = true;
			}
		}
		NunAlertManager.getInstance().AddNun(this,true);
		if(redLight!=null) redLight.enabled=true;
	}
	
	protected void deactivateChaseElements(){
		chaseMusic.GetComponent<MusicManager>().reduceVolumeTemp();
		foreach(MotionBlur b in cameraMotionBlur){
			b.enabled = false;
		}
		NunAlertManager.getInstance().RemoveNun(this, false);			
		NunAlertManager.getInstance().RemoveNun(this, true);	
		
		if(redLight!=null) redLight.enabled=false;
		
		if(GetComponentInChildren<DisappearAfterSuccessfulHiding>()!=null){
			//agent.SetDestination(GetComponentInChildren<DisappearAfterSuccessfulHiding>().gameObject.transform.position);
			Invoke("destroyNunAfterChase",5.0f);
		}		
		
	}
	
	private void destroyNunAfterChase(){
		Destroy(gameObject);
	}
	
	protected IEnumerator ChasingKid_EnterState()
	{
		agent.Resume();
		agent.speed = chaseSpeed;
		nunScream.Play();
		
		//this will revert when a nun exits its state
		activateChaseElements();
		
		InvokeRepeating("SetKidDestination", 0.1f, 1f);
		yield return null;
	}
	
	protected void ChasingKid_Update()
	{
		PlayRunningAudioClip();
		float distanceToKid = Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.z) 
		                                           - new Vector2(kid.transform.position.x, kid.transform.position.z));
		if(distanceToKid < stopDistanceOnCatch * stopDistanceOnCatch) //alternatives: raycast & agent.remaining distance
		{
			agent.Stop(true);
			CancelInvoke("SetKidDestination");
			kid.GetComponent<TP_Controller>().removeControl();
			currentState = NunStates.CaughtKid;
		}
		
		if (hidingController.hiding) {
			hidingSpots = hidingController.GetHidingSpotsInRange ();
			Debug.Log ("hiding count: " + hidingSpots.Count);
			RaycastHit hit;
			int layerMask = 1 << LayerMask.NameToLayer ("Player");
			layerMask += 1 << LayerMask.NameToLayer ("Wall");
			if (Physics.Raycast (transform.position, kid.transform.position - transform.position, out hit, 
			                     investigationRange, layerMask)) {
				if (hit.transform.tag == kid.tag)
					currentState = NunStates.SawKidHiding;
				else
					currentState = NunStates.MissedKidHiding;
			} else
				currentState = NunStates.MissedKidHiding;
			CancelInvoke ("SetKidDestination");
		}
		//}
	}
	#endregion
	
	#region CaughtKid
	protected IEnumerator CaughtKid_EnterState()
	{
		lookRotation = Quaternion.LookRotation( (transform.position + new Vector3(0, agent.height, 0)) 
		                                       - kid.transform.position);
		transform.forward = kid.transform.position - transform.position;
		transform.forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
		mashTime = mashingLength * 0.3f;
		//StartCoroutine("FlashInteractionKey");
		yield return null;
	}
	
	protected void CaughtKid_Update()
	{
		kid.transform.rotation = Quaternion.Slerp(kid.transform.rotation, lookRotation, 4f * Time.deltaTime);
		Vector3 localAngles = cameraTransform.localEulerAngles;
		cameraTransform.localEulerAngles = new Vector3(Mathf.LerpAngle(localAngles.x, 0, 4f * Time.deltaTime),
		                                               localAngles.y, localAngles.z);		
		
		if(Input.GetButtonDown("Mash"))
		{
			mashTime -= Time.deltaTime;
			lastButtonPress = Time.time;
		}
		else if(Time.time - lastButtonPress < Time.deltaTime * 5f)
			mashTime -= Time.deltaTime;
		else
			mashTime += Time.deltaTime;
		
		mashRatio = Mathf.Clamp(mashTime, 0f, mashingLength) / mashingLength;
		
		nunModel.transform.localEulerAngles = new Vector3(30f * mashRatio, nunModel.transform.localEulerAngles.y,
		                                                  nunModel.transform.localEulerAngles.z);
		if(mashRatio == 0)
		{
			StopCoroutine("FlashInteractionKey");
			currentState = NunStates.Stuned;
		}
		else if(mashRatio == 1)
		{
			if(GetComponentInChildren<LostWithThisNun>()!=null) GetComponentInChildren<LostWithThisNun>().SaveLoss();
			gameManager.ResetLevel();
		}
	}
	
	protected void CaughtKid_OnGUI()
	{	
		if(displayKey)
			GUI.DrawTexture(new Rect(20, 20, 250, 100), gameManager.interactTexture);
	}
	
	#endregion
	
	#region Stuned
	
	protected IEnumerator Stuned_EnterState()
	{
		lookRotation = Quaternion.LookRotation(transform.forward);
		// wait 1 sec for the camera to turn around and give control back to player
		yield return new WaitForSeconds(1);
		kid.transform.rotation = transform.rotation;
		kid.GetComponent<TP_Controller>().returnControl();
		// after a few seconds the nun starts chasing again
		yield return new WaitForSeconds(timeToWaitAfterStun - 1);
		currentState = NunStates.ChasingKid;
		yield return null;
	}
	
	protected void Stuned_Update()
	{
		if(kid.GetComponent<TP_Controller>().hasControl == false)
		{
			kid.transform.rotation = Quaternion.Slerp(kid.transform.rotation, lookRotation, 4f * Time.deltaTime);
			Vector3 localAngles = cameraTransform.localEulerAngles;
			cameraTransform.localEulerAngles = new Vector3(Mathf.LerpAngle(localAngles.x, 0, 4f * Time.deltaTime),
			                                               localAngles.y, localAngles.z);
		}
	}
	#endregion
	
	#region SawKidHiding
	
	protected IEnumerator SawKidHiding_EnterState()
	{
		if(hidingSpots.Count == 0)
		{
			currentState = NunStates.GetsKidOut;
			yield break;
		}	
		currentNode = (int)Random.Range(0, hidingSpots.Count);
		agent.SetDestination(hidingSpots[currentNode]);
		hidingSpots.RemoveAt(currentNode);
		isWaiting = false;
		agent.speed = patrolSpeed;
		yield return null;
	}
	
	protected void SawKidHiding_Update()
	{
		DoRoutine(NunStates.GetsKidOut);
	}
	#endregion
	
	#region MissedKidHiding
	
	protected IEnumerator MissedKidHiding_EnterState()
	{	
		//add kid hiding spot just to scare kid
		hidingSpots.Add(hidingController.GetCurrentHidingSpot());
		// since we always add the current hiding place the count can't be 0
		//		if(hidingSpots.Count == 0)
		//		{
		//			deactivateChaseElements();
		//			ActivateDistractionInvestigation(transform);			
		//			yield break;
		//		}
		
		currentNode = (int)Random.Range(0, hidingSpots.Count);
		agent.SetDestination(hidingSpots[currentNode]);
		hidingSpots.RemoveAt(currentNode);
		isWaiting = false;
		agent.speed = patrolSpeed;
		yield return null;
	}
	
	protected void MissedKidHiding_Update()
	{
		DoRoutine(NunStates.Default);
	}
	#endregion
	
	#region GetsKidOut
	
	protected IEnumerator GetsKidOut_EnterState()
	{
		agent.SetDestination(kid.transform.position);
		yield return null;	
	}
	
	protected void GetsKidOut_Update()
	{
		if(hidingController.hiding == false)
			currentState = NunStates.ChasingKid;
		
		float distanceX = Mathf.Abs(transform.position.x - agent.destination.x);
		float distanceZ = Mathf.Abs(transform.position.z - agent.destination.z);
		
		if (distanceX <= agent.stoppingDistance && distanceZ <= agent.stoppingDistance)
		{
			agent.Stop();
			if(hidingController.hiding){
				//hidingController.hidingSpot.DisableHidingSpot();
				hidingController.ComeOut(transform);			
			}
		}
	}
	#endregion
	
	// !!!! possible addition -> check if the kid is sneaking (for invest range and/or chase range)
	private bool CheckIfKidInRange()
	{
		LineOfSight range = SeesKid();
		
		if(range == LineOfSight.NotInRange)
			return false;
		else
		{
			investigatingLocation = kid.transform.position;
			if(range == LineOfSight.ChaseRange)
				currentState = NunStates.ChasingKid;
			else if(range == LineOfSight.InvestigationRange)
				currentState = NunStates.Investigating;
			return true;
		}
	}
	
	protected LineOfSight SeesKid()
	{
		RaycastHit hit;	
		Vector3 targetDir = kid.transform.position - transform.position;
		float angle= Vector3.Angle(targetDir, transform.forward);
		
		if (angle > viewAngle) //if outside cone of vision
			return LineOfSight.NotInRange;
		if(Physics.Raycast(transform.position, targetDir, out hit, Mathf.Infinity))
		{
			if(hit.collider == kid.collider)
			{
				if (hit.distance <= chaseRange) //if in chase range
					return LineOfSight.ChaseRange;
				else if(hit.distance <= investigationRange)
					return LineOfSight.InvestigationRange;
			}
		}
		return LineOfSight.NotInRange;
	}
	
	protected void LookAround()
	{
		if (Quaternion.Angle(transform.rotation,relativeForwardRotation) >= maxRotation) 
			rotatePositive = !rotatePositive;
		if (rotatePositive)
			transform.Rotate(0, 5 * Time.deltaTime, 0);
		else 
			transform.Rotate(0, -5 * Time.deltaTime, 0);
	}
	
	IEnumerator GoToState(float timeToWait, NunStates state)
	{
		yield return new  WaitForSeconds(timeToWait);
		currentState = state;
		yield return null;
	}
	
	public virtual void ActivateDistractionInvestigation(Transform location)
	{
		if(CurrentStateEqualTo(NunStates.Default) || CurrentStateEqualTo(NunStates.Investigating))
		{
			investigatingLocation = location.position;
			currentState = NunStates.Investigating;
		}
	}
	
	public bool CurrentStateEqualTo(NunStates state)
	{
		if(currentState.GetType() != state.GetType()) return false;
		
		return currentState.CompareTo(state as System.Enum) == 0;
	}
	public bool CurrentStateEqualTo(SleepingStates state)
	{
		if(currentState.GetType() != state.GetType()) return false;
		
		return currentState.CompareTo(state as System.Enum) == 0;
	}
	// Invoked every 1 sec so the path is not re-calculated on every frame	
	private void SetKidDestination()
	{
		agent.SetDestination(kid.transform.position);		
	}
	
	private IEnumerator FlashInteractionKey()
	{
		while(true)
		{
			displayKey = true;
			yield return new WaitForSeconds(0.5f);
			displayKey = false;
			yield return new WaitForSeconds(0.5f);
		}
	}
	
	IEnumerator SetNextNode(float timeToWait, NunStates stateToGo)
	{ 
		yield return new  WaitForSeconds(timeToWait);
		if(hidingSpots.Count == 0)
		{
			//Reverting chase music and feedback
			Invoke("deactivateChaseElements",3.0f);
			
			currentState = stateToGo;
		}
		else
		{
			currentNode = (int)Random.Range(0, hidingSpots.Count);
			agent.destination = hidingSpots[currentNode];
			hidingSpots.RemoveAt(currentNode);
		}
		isWaiting = false;
		yield return null;
	}
	
	private void DoRoutine(NunStates nextState)
	{
		if(hidingController.hiding == false)
			currentState = NunStates.ChasingKid;
		
		float distanceX = Mathf.Abs(transform.position.x - agent.destination.x);
		float distanceZ = Mathf.Abs(transform.position.z - agent.destination.z);
		
		if (distanceX <= agent.stoppingDistance && distanceZ <= agent.stoppingDistance && !isWaiting)
		{	
			isWaiting = true;
			float waitTime = Random.Range(0f, 3f);
			
			StartCoroutine(SetNextNode(waitTime, nextState));
		}
		if(!isWaiting)
			PlayWalkingAudioClip();
		else
			audioSource.Stop();
	}
	
	protected void PlayWalkingAudioClip()
	{
		if(audioSource.isPlaying == false)
		{
			audioSource.clip = audioManager.nunWalkingSteps;
			audioSource.Play();	
		}
	}
	
	protected void PlayRunningAudioClip()
	{
		if(audioSource.isPlaying == false)
		{
			audioSource.clip = audioManager.nunRunningSteps;
			audioSource.Play();	
		}
	}
}
