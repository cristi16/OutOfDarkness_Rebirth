using UnityEngine;
using System.Collections;

public class SleepingNun : AI {
	
	public bool scared=false;
	private float wait_after_scare_time = 10;
	private float scare_distance_before_stopping = 10;
	public GameObject safeSpot;	
	private float audioVolume;
	private AudioSource audio;
	
	public override void Start(){
		base.Start();
		
		GameObject temp = transform.FindChild("Sleeping Nun Sound").gameObject;
		
		if(temp == null){
			Debug.LogError("Error inizialization of agent, Sleeping Nun script "+this.name);
			return;
		}
		
		audio = temp.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (scared){
			if(audio.enabled)
				turnSnoreOff();
			
			Scare();
		}
		else {
			if (!isSleeping()){
				if(!base.light.enabled)
					turnLightOn();
				
				if(audio.enabled)
					turnSnoreOff();
				
				base.Update();
				base.setInvestigatingDistance(base.distance_default); //Cheat (calling the base it resets the investigating to 0 for no reason)
			}else{
				if(base.light.enabled)
					turnLightOff();
				
				if(!audio.enabled)
					turnSnoreOn();
				
				Sleep();
				GetComponentInChildren<AudioSource>().volume=1f;							
			}
		}	
		// Debugging FoV
		Debug.DrawLine(transform.position, transform.position + transform.forward * viewDistance, lineCastColor);
		Quaternion rotation = Quaternion.Euler(new Vector3(0, viewAngle, 0));	
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward *  viewDistance, lineCastColor);
		rotation = Quaternion.Euler(new Vector3(0, -viewAngle, 0));
		Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * viewDistance, lineCastColor);
	}
	
	void Sleep() {
		Vector2 nunPos = new Vector2(transform.position.x, transform.position.z);
		Vector2 staticSpot = new Vector2(base.getStaticSpot().x, base.getStaticSpot().z);
		if (Vector2.Distance(nunPos, staticSpot) > 0.1f){
			base.getAgent().destination=base.getStaticSpot();
			reachedStaticPoint = false;
		}else {
			transform.rotation = Quaternion.Slerp(transform.rotation, base.getStaticRotation(), Time.deltaTime * 2);
			
			// triggers sitting animations
			if(!reachedStaticPoint)
			{
				setSitting(true);
				reachedStaticPoint = true;
			}
		}
	}
	
	bool isSleeping(){
		if(base.getInvest() || base.getChase()){
			return false;
		}else{
			return true;
		}
	}
	
	public void Scare(){
		if (Vector3.Distance(safeSpot.transform.position, transform.position)>= scare_distance_before_stopping){
			
			if(!scared)
				setStanding(true);
			
			scared=true;						
			
			base.getAgent().destination=safeSpot.transform.position;
		}else {
			base.getAgent().Stop();
			
			base.lookAround();
			
			if(!base.checkTimer())
				scared=false;
		}
	}
	
	public bool getScared(){
		return scared;
	}
	
	public void setScareDistance(float temp){
		scare_distance_before_stopping = temp;
	}
	
	public void setTimeAfterScare(float temp){
		wait_after_scare_time = temp;
	}
	
	public void turnLightOn(){
		light.enabled = true;
	}
	
	public void turnLightOff(){
		light.enabled = false;
	}
	
	public void turnSnoreOff(){
		audio.enabled = false;
	}
	
	public void turnSnoreOn(){
		audio.enabled = true;
	}
}
