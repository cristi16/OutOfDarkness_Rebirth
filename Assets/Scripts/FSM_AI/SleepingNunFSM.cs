using UnityEngine;
using System.Collections;

public class SleepingNunFSM : NunStateMachine {
	
	public float deadZoneCaughing = 2f;
	public float coolDownCaughing = 5f;
	
	private Vector3 static_spot;
	private Quaternion static_rotation;
	//ANIMATION
	protected bool standing = false;
	protected bool sitting = false;
	
	private float lastCaughTime = 0f;
	private AudioSource sleepingAudioSource;
	private AudioSource alertAudioSource;
	
	protected override void OnAwake()
	{
		static_spot = transform.position;
		static_rotation = transform.rotation;
		sleepingAudioSource = transform.FindChild("Sleeping Audio Source").GetComponent<AudioSource>();
		alertAudioSource = GameObject.FindGameObjectWithTag("Audio").audio;
		base.OnAwake();
	}
	
	#region Sleeping
	protected override IEnumerator Default_EnterState()
	{
		agent.destination = static_spot;
		relativeForwardRotation = static_rotation;
		agent.speed = patrolSpeed;
		agent.stoppingDistance = 0f;
		sleepingAudioSource.clip = audioManager.nunSnoring;
		sleepingAudioSource.Play();
		yield return null;	
	}
		
	protected override void Default_Update() 
	{
		Vector2 nunPos = new Vector2(transform.position.x, transform.position.z);
		Vector2 staticSpot = new Vector2(static_spot.x, static_spot.z);
		
		
		if(Vector2.Distance(nunPos, staticSpot) < agent.stoppingDistance + 0.3f)
		{
			if(Quaternion.Angle(transform.rotation, static_rotation) > 2f)
				transform.rotation = Quaternion.Slerp(transform.rotation, static_rotation, Time.deltaTime * 2);
			sitting = true;		
		}
	}
	
	protected override IEnumerator Default_ExitState()
	{	
		yield return null;
	}
	#endregion
	
	#region Caughing
	
	protected IEnumerator Caughing_EnterState()
	{
		sleepingAudioSource.Stop();
		sleepingAudioSource.clip = audioManager.nunCaughing;
		sleepingAudioSource.Play();
		
		//alertAudioSource.clip = audioManager.nunFirstAlert;
		//alertAudioSource.Play();
		
		
		yield return new WaitForSeconds(coolDownCaughing);
		if(CurrentStateEqualTo(SleepingStates.Caughing))
		{
			currentState = NunStates.Default;
		}
		yield return null;
	}
	
	protected void Caughing_Update()
	{
		
	}
	
	protected IEnumerator Caughing_ExitState()
	{
		yield return null;
	}
	#endregion
	
	public override void ActivateDistractionInvestigation(Transform location)
	{
//////// Removed to avoid looping through investigating state and causing the nun to not go to chase when she is close to the kid
//		if(CurrentStateEqualTo(NunStates.Investigating))
//		{
//			investigatingLocation = location.position;
//			currentState = NunStates.Investigating;
//		}
//		else 
/////////////////
		if(CurrentStateEqualTo(NunStates.Default))
		{
			lastCaughTime = Time.time;
			currentState = SleepingStates.Caughing;
		}
		else if(CurrentStateEqualTo(SleepingStates.Caughing) && (Time.time - lastCaughTime) > deadZoneCaughing)
		{
			agent.stoppingDistance = defaultStoppingDistance;
			sleepingAudioSource.Stop();
			sitting = false;
			standing = true;
			investigatingLocation = location.position;
			currentState = NunStates.Investigating;
		}
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
}
