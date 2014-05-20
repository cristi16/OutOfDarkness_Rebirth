using UnityEngine;
using System.Collections;

public class Scare : MonoBehaviour {
	
	public float wait_after_scare_time = 10;
	public float scare_distance_before_stopping = 10;
	public SleepingNun[] nuns; 
	
	private AudioSource audioSource;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void scareNunsArray(){
		if(nuns.Length != 0){
			for(int i = 0; i < nuns.Length; i++){
				if(!nuns[i].getScared()){
					//Debug.Log(nuns[i].name + "scared");
					nuns[i].setTimeAfterScare(wait_after_scare_time);
					nuns[i].setScareDistance(scare_distance_before_stopping);
					nuns[i].Scare();
				}
			}
		}
		changeParticlesColor();
		playFeedbackSound();
	}
	
	public void changeParticlesColor(){
		transform.parent.GetComponentInChildren<DistractionParticleController>().changeParticlesColor(wait_after_scare_time);
	}
		
	public void playFeedbackSound(){
		if(audioSource!=null) audioSource.Play();
	}
}
