using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour {
	
	public bool fadeIn=false;
	private bool executed=false;	
	
	private float timeToFade=1f;
	private float time=0f;		
	
	public MusicManager musicManager;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider other){		
		if(executed) return;
		
		if(other.tag=="Kid"){ 
			if(fadeIn){
				musicManager.augmentVolume();
			} else {
				musicManager.reduceVolumeTemp();
			}			
			executed=true;
		}
	}
}
