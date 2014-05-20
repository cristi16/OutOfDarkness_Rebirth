using UnityEngine;
using System.Collections;

public class MusicEnablerDisabler : MonoBehaviour {
	
	public float timeToWaitBeforeFade=1.0f;
	
	internal bool fadeOut=false;
	internal bool fadeIn=false;
	
	private float time=0f;
	public MusicManager musicManager;
	
	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
		if(fadeOut || fadeIn){
			time+=Time.deltaTime;
			if(time>=timeToWaitBeforeFade){
				//Fade
				if(fadeOut) musicManager.reduceVolumeTemp();
				if(fadeIn) musicManager.augmentVolume();
				
				fadeOut=false;
				fadeIn=false;
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			fadeIn=true;
			fadeOut=false;
			time=0f;
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.tag=="Kid"){
			fadeOut=true;
			fadeIn=false;
			time=0f;
		}
	}
}
