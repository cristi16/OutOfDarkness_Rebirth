using UnityEngine;
using System.Collections;

public class SneakWalkRunController : MonoBehaviour {
	
	public float sneakingMultiplier = 0.7f;
	public float runningMultiplier = 1.4f;
	
	private bool sneak = false;
	private bool run = false;
	
	private TP_Motor motor;
	private float lastRelease = 0f;	
	private LevelState level;
	private float previousSneakAxis=0f;
	private float previousRunAxis=0f;
	
	// Use this for initialization
	void Start () {
		motor = gameObject.GetComponent<TP_Motor>();		
		level = LevelState.getInstance();
	}
	
	// Update is called once per frame
	void Update () {						
		Debug.Log (Input.GetAxis("Run"));

		bool stopMethod = false;
		if((Input.GetButtonUp("Run") || (previousRunAxis!=0f && Input.GetAxis("Run")==0f)))
		{
			lastRelease = Time.time;
			stopMethod=true;
		} else if((Input.GetButtonUp("Sneak") || (previousSneakAxis!=0f &&  Input.GetAxis("Sneak")==0f)))
		{
			lastRelease = Time.time;
			stopMethod=true;
		}

		previousSneakAxis = Input.GetAxis ("Sneak");
		previousRunAxis = Input.GetAxis ("Run");

		if (stopMethod)
						return;


		if((Input.GetButton("Run") || Input.GetAxis("Run")>0.5f) && level.runActivated)
		{
			run=true;
			sneak=false;
		}
		else
		{
			if(((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f)&&level.sneakActivated) || motor.moveVector == Vector3.zero)
			{
				sneak = true;
				run = false;
			}
			else
			{
				if( (Time.time - lastRelease) > 0.1f ){
					sneak = false;
					run=false; 
				}
			}
		}		
	}
	
	public bool getSneak(){
		return sneak;
	}
	
	public void setSneak(bool temp){
		sneak = temp;
	}
	
	public bool getRun(){
		return run;
	}
	
	public void setRun(bool temp){
		run = temp;
	}
	
	public bool isStatic(){
		return (motor.moveVector == Vector3.zero);
	}
}
