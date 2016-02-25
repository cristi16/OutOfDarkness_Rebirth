using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	
	private AudioManager audioManager;
	private AudioSource audioSource;
	private NavMeshAgent agent;
	private GameObject player;
	public float viewAngle = 30.0F;
	public float viewDistance = 20.0F;
	public bool patrol;
	public GameObject[] nodes;
	public int currentNode=0;
	public float patrol_speed=3.5f;
	public float chase_speed=5f;
	public int max_rotation=20;
	private bool rotation_direction=true;
	private bool front=true;
	private bool inverse=false;
	protected Vector3 static_spot;
	private Quaternion static_rotation;
	private Quaternion looking_direction=Quaternion.identity;
	private Quaternion looking_direction_opposite=Quaternion.identity;
	//private CheckpointsManager_Script gameManager;
	private NunChaseInvestigateManager nunManager;
	
	private bool chaseOn=false;
	
	//ANIMATION
	private bool waiting = false;
	protected bool standing = false;
	protected bool sitting = false;
	protected bool reachedStaticPoint = true;
	
	//Human behaviour (sort of)
	public float multiplier_while_waiting = 1.2f;
	public float multiplier_while_walking = 8;
	public float random_wait_min_time = 8;
	private float next_random_wait;
	private float timer_between_random_wait;
	
	//Checking if the nun see the player variables
	internal float playerHalfHeight;
	internal float playerHalfWidth;
	
	//Investigating Variables
	internal float distance_default = 0;
	private float wait_after_investigating_time = 10f; //Time that you wait after being distracted
	private GameObject investigating_location;
	private bool isInvestigating=false;
	private bool timerSetted = false;
	private float endTime;
	private bool was_chasing=false;
	private bool isGoingBack=false;
	
	//Debug Variables
	public Color lineCastColor = Color.white;
	
	//Light/Sound Variables
	internal Light light;
	
	private AudioSource nunScream;
	
	
	public virtual void Start () {
		
		//GET COMPONENTS
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Kid");
		
				//gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CheckpointsManager_Script>();
		nunManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<NunChaseInvestigateManager>();
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		audioSource = gameObject.GetComponent<AudioSource>();
		GameObject temp = transform.FindChild("Spotlight").gameObject;
		
		if(agent == null || player == null || temp == null){
			Debug.LogError("Error inizialization of agent, AI script "+this.name);
			return;
		}
		
		//SETTING VARIABLES
		if (!patrol) {
			static_spot=transform.position;
			static_rotation=transform.rotation;
		}
		
		light = temp.GetComponent<Light>();
		
		playerHalfHeight = player.GetComponent<CharacterController>().height / 2;
		playerHalfWidth = player.GetComponent<CharacterController>().radius;	

		agent.stoppingDistance = distance_default;
		agent.speed = patrol_speed;
		
		lineCastColor = Color.white;
		
		getRotation();
		
		nunScream = GameObject.FindGameObjectWithTag("Nun Scream").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	public virtual void Update () {	
		
		if (CanSee()){ //If the nun can see the player AI: START CHASING
			if (!chaseOn) //Is not chasing already
				activateChase();
			
			Chase();
			
		}else{
			if(chaseOn){ //She was chasing AI: START INVESTIGATING
				deactivateChase();
				activateChasingInvestigate(player);
			}else if(isInvestigating){
				Investigate();				
			}else{
				if (patrol)
					Patrol ();
				else 
					Static();
			}
		}
		
		// Debugging FoV
		Debug.DrawLine(transform.position, transform.position + transform.forward * viewDistance, lineCastColor);
		Quaternion rotation = Quaternion.Euler(new Vector3(0, viewAngle, 0));	
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward *  viewDistance, lineCastColor);
		rotation = Quaternion.Euler(new Vector3(0, -viewAngle, 0));
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * viewDistance, lineCastColor);
	}
	
	// States: static, patrol, chase, investigate, scared?
	
	void Static() {
		if(agent.velocity == Vector3.zero){ //Has reached the static point
			if (isGoingBack){ //Has reached the static point
				transform.rotation = Quaternion.Slerp(transform.rotation, static_rotation, Time.deltaTime * 2);
				
				if(transform.rotation == static_rotation)
					isGoingBack = false;
				
				if(reachedStaticPoint){
					// triggers sitting animations
					//nunManager.RemoveNun(this,true);
					//nunManager.RemoveNun(this,false);
					
					setSitting(true);
					
					reachedStaticPoint = false;	
				}
			}else { //Looking left and right while static
				if(!reachedStaticPoint)
					reachedStaticPoint = true;
			
				lookAround();
			}
		}
	}
	
	void Patrol() {
		if(!waiting){
			float distanceX = Mathf.Abs(transform.position.x - agent.destination.x);
			float distanceZ = Mathf.Abs(transform.position.z - agent.destination.z);

			if (distanceX <= agent.stoppingDistance && distanceZ <= agent.stoppingDistance && !isInvestigating){	
				
				//nunManager.RemoveNun(this,true);
				//nunManager.RemoveNun(this,false);
				
				if (++currentNode == nodes.Length)
					currentNode=0;
				
				if(isGoingBack){
					isGoingBack = false;
				}
			}
			agent.destination=nodes[currentNode].transform.position;
			
			PlayWalkingAudioClip(); //AUDIO: Walking
		}
		
		if(!waiting && Time.time > next_random_wait){
			//Debug.Log(this.name +" stopped");
			agent.Stop();
			waiting=true;
			timer_between_random_wait = Time.time + multiplier_while_waiting*Random.value +1;
			next_random_wait = Time.time + multiplier_while_walking*Random.value + random_wait_min_time + timer_between_random_wait;
		}
		
		if(waiting && Time.time > timer_between_random_wait){
			agent.Resume();
			//Debug.Log(this.name +" resume");
			waiting=false;
		}
	}
	
	public void activateChase(){
		
		nunManager.AddNun(this,true);
		agent.speed=chase_speed;
		
		checkSittingAnimation(); //ANIMATION: check if she has to stand up
			
		chaseOn = true;
		
		//Debug.Log("AI: "+transform.name + " starts chasing");
	}

	private void Chase() {
		agent.destination=player.transform.position;
		PlayRunningAudioClip(); //AUDIO: Running
	}

	private void deactivateChase(){
		was_chasing = true;
		chaseOn = false;
		//nunManager.RemoveNun(this,true);
		
		//Debug.Log("AI: "+transform.name + " has finished chasing");
		Invoke ("removeNunChase",3.0f);
	}
	
	public void activateNormalInvestigate(GameObject location,float distance = -1, bool activateVisualFeedback=false) {
		checkSittingAnimation();
		
		//run_for_invest = true;
		isInvestigating = true;		
		
		if(distance != -1)
			setInvestigatingDistance(distance);
		
		if(player.transform==location || activateVisualFeedback) nunManager.AddNun(this,false);				
						
		if(waiting) waiting=false;				
		
		agent.destination=location.transform.position;
		investigating_location=location;
		
		agent.Stop();
		agent.ResetPath();		
		agent.Resume();
		
		nunScream.Play(44100);
		
		//Debug.Log("AI: "+transform.name + " normal Investigate");
	}
	
	public void activateChasingInvestigate(GameObject location, float distance = -1, bool activateVisualFeedback=false) {
		checkSittingAnimation();
		
		//run_for_invest = true;
		isInvestigating = true;
		
		if(distance != -1)
			setInvestigatingDistance(distance);
		
		agent.speed = chase_speed;
		was_chasing = true;
		waiting=false;
		
		if(activateVisualFeedback) nunManager.AddNun(this,false);
				
		if(waiting) waiting=false;				
		
		agent.destination=location.transform.position;
		investigating_location=location;
		
		agent.Stop();
		agent.ResetPath();		
		agent.Resume();
		
		nunScream.Play(44100);
		
		
		//.Log("AI: "+transform.name + " chasing Investigate");
	}
	
	private void Investigate(){
		//The investigation point never moves so the destination is never updated but in the activateInvestigate
	
		PlayWalkingAudioClip();
		
		//if (agent.velocity == Vector3.zero){ //distraction reached
		
		float distanceX = Mathf.Abs(transform.position.x - agent.destination.x);
		float distanceZ = Mathf.Abs(transform.position.z - agent.destination.z);

		if (distanceX <= agent.stoppingDistance && distanceZ <= agent.stoppingDistance){ //distraction reached
			
			checkTimer();
			
			if (!was_chasing) {
				lookAround();
			}else { // 360 degrees
				lookAroundFull();
			}
			

		} else {
			agent.destination = investigating_location.transform.position;
		}
	}
		
	private void removeNunInvestigate(){
		nunManager.RemoveNun(this,false);
	}
	
	private void removeNunChase(){
		nunManager.RemoveNun(this,true);
	}
	
	private void deactivateInvestigate(){
			
		//Debug.Log("AI: " + transform.name + " has finished investigate");
		was_chasing=false;
		isInvestigating = false;
		//nunManager.RemoveNun(this,false);
		agent.speed=patrol_speed;
		looking_direction=Quaternion.identity;
		
		Invoke ("removeNunInvestigate",3.0f);
		
		isGoingBack=true;
			
		if (patrol){ //If it was patrolling AI: GO TO THE NEAREST NODE
			int closest_node=0;
			float d=999999f;
			for (int i=0; i < nodes.Length; i++){
				float temp = Vector3.Distance(nodes[i].transform.position, transform.position);
				if (temp < d) {
					d=temp;
					closest_node=i;
				}
			}
					
			currentNode=closest_node;
		}else{ //If it was static AI: GO TO THE START POINT
			agent.destination = static_spot;
		}

	}
	
	internal bool checkTimer(){
		if (!timerSetted){ //Reset the Endtime (Called just once)
			getRotation();
			endTime = Time.time + wait_after_investigating_time;
			timerSetted=true;
		}else {
			if (endTime <= Time.time){
				deactivateInvestigate();
				timerSetted=false;
				setInvestigatingDistance(distance_default);				
			}
		}
		
		return timerSetted; //return false if it's finished, 'cause the first time it's called it starts
	}
	
	private void getRotation(){
	
		looking_direction=transform.rotation;
		looking_direction_opposite= transform.rotation;
		looking_direction_opposite.Set(looking_direction.x, (looking_direction.y+180f),looking_direction.z, looking_direction.w);
	}
	
	internal void lookAround(){
		if (Quaternion.Angle(transform.rotation,looking_direction)>=max_rotation) 
			rotation_direction=!rotation_direction;
		if (rotation_direction)
			transform.Rotate(0,5*Time.deltaTime,0);
		else 
			transform.Rotate(0,-5*Time.deltaTime,0);
	}
	
	private void lookAroundFull(){
		if (front && Quaternion.Angle(transform.rotation,looking_direction)>=max_rotation) {
			rotation_direction=!rotation_direction;
			if (rotation_direction)
				front=false;
		}
			
		if (Quaternion.Angle(transform.rotation,looking_direction)>=170 && Quaternion.Angle(transform.rotation,looking_direction)<=180) {
			looking_direction_opposite=transform.rotation;
			inverse=true;
		}

		if (inverse && !front && Quaternion.Angle(transform.rotation,looking_direction_opposite)>=(max_rotation)) {
			rotation_direction=!rotation_direction;
			if (rotation_direction) {
				front=false;
				inverse=false;
			} 
		}

		if (rotation_direction && front)
			transform.Rotate(0,50*Time.deltaTime,0);
		else if (!rotation_direction && front) {
			transform.Rotate(0,-50*Time.deltaTime,0);
		}
		else if (rotation_direction && !front)
			transform.Rotate(0,-50*Time.deltaTime,0);
		else if (!rotation_direction && !front) {
			transform.Rotate(0,50*Time.deltaTime,0);
		}
	}

	private void checkSittingAnimation(){
		if(!isInvestigating){
			Vector2 nunPos = new Vector2(transform.position.x, transform.position.z);
			Vector2 staticSpot = new Vector2(static_spot.x, static_spot.z);
			
			if(Vector2.Distance(nunPos, staticSpot) < 1f)
			{
				setStanding(true);	
			}
		}
	}
	
	bool CanSee() { //Check if the nun can see the player
		
		RaycastHit hit;
		Vector3 targetDir = (player.transform.position + new Vector3(-playerHalfWidth, playerHalfHeight, 0) ) - transform.position;
		if(Physics.Raycast(transform.position, targetDir, out hit, Mathf.Infinity)){
			if(hit.collider==player.GetComponent<Collider>()){
				if (hit.distance <= viewDistance){ //if in range
			        Vector3 forward = transform.forward;	
			        float angle= Vector3.Angle(targetDir, forward);
			        if (angle < viewAngle) //if inside cone of vision
			            return true;
				}
			}
		}
		
//		Vector3 targetDirRight = (player.transform.position + new Vector3(-playerHalfWidth, playerHalfHeight, 0) ) - transform.position;
//        Vector3 targetDirLeft = (player.transform.position + new Vector3(playerHalfWidth, playerHalfHeight, 0) ) - transform.position;
//		
//
//		
//		if (Physics.Raycast(transform.position,  targetDirRight, out hit) || Physics.Raycast(transform.position,  targetDirLeft, out hit)){ //if there is no obstacle
//			if(hit.collider==player.collider){
//				
//				Debug.DrawRay(transform.position, targetDirLeft);
//				Debug.DrawRay(transform.position, targetDirRight);
//				if (hit.distance <= viewDistance){ //if in range
//			        Vector3 forward = transform.forward;	
//			        float angleLeft = Vector3.Angle(targetDirLeft, forward);
//					float angleRight = Vector3.Angle(targetDirRight, forward);
//			        if (angleLeft < viewAngle || angleRight < viewAngle) //if inside cone of vision
//			            return true;
//				}
//			}
//		}
		return false;
	}
	
	public bool getInvest(){
		return isInvestigating;
	}
	
	public bool getChase(){
		return chaseOn;
	}
	
	public NavMeshAgent getAgent(){
		return agent;
	}
	
	public Quaternion getStaticRotation(){
		return static_rotation;
	}
	
	public Vector3 getStaticSpot(){
		return static_spot;
	}
	
	public Quaternion getLookingDirection(){
		return looking_direction;
	}
	
	public bool getRotationDirection(){
		return rotation_direction;
	}
	
	public float getEndTime(){
		return endTime;
	}
	
	public void setLookingDirection(Quaternion temp){
		looking_direction = temp;
	}
	
	public void setRotationDirection(bool temp){
		rotation_direction = temp;
	}
	
	public void setEndTime(float temp){
		endTime = temp;
	}
	
	public void setPlayer(GameObject temp){
		player = temp;
	}
	
	public void setInvest(bool temp){
		isInvestigating = temp;
	}
	
	public void setTimeAfterDistraction(float temp){
		wait_after_investigating_time = temp;
	}
	
	public void setInvestigatingDistance(float temp){
		agent.stoppingDistance = temp;
	}
	
	public bool getSitting(){
		return sitting;
	}
	
	public bool getStanding(){
		return standing;
	}
	
	public void setStanding(bool temp){
		standing=temp;
	}
	
	public void setSitting(bool temp){
		sitting=temp;
	}

	public bool getGoingBack(){
		return isGoingBack;
	}
	
	private void PlayWalkingAudioClip()
	{
		if(audioSource.isPlaying == false)
		{
			audioSource.clip = audioManager.nunWalkingSteps;
			audioSource.Play();	
		}
	}
	
	private void PlayRunningAudioClip()
	{
		if(audioSource.isPlaying == false)
		{
			audioSource.clip = audioManager.nunRunningSteps;
			audioSource.Play();	
		}
	}
}
