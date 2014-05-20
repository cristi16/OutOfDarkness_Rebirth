using UnityEngine;
using System.Collections;

public class Distraction : MonoBehaviour {
	
	public float distraction_distance_before_stopping = 10;
	public float waiting_time_after_distraction = 10;
	private bool distraction_enabled;
	private GameObject closestNun = null;
	private AI nunAI;
	private AudioSource audioSource;
	
	public AI[] nuns; 
	
	// Use this for initialization
	void Start () {
		distraction_enabled = true;
		closestNun = null;
		nunAI = null;		
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void distractNearest(){
		if(closestNun == null || !nunAI.getInvest()){
			//Debug.Log("Distraction activated");
			closestNun = null;
			GameObject[] nuns = GameObject.FindGameObjectsWithTag("Nun");
			float distance=999999f;
			if(nuns.Length > 0){
				for (int i=0; i < nuns.Length; i++){
					if (Vector3.Distance(nuns[i].transform.position, transform.position) <= distance){
						distance= Vector3.Distance(nuns[i].transform.position, transform.position);
						//Debug.Log(distance);
						closestNun=nuns[i];
					}
				}
				nunAI = closestNun.GetComponent<AI>();
				nunAI.activateNormalInvestigate(transform.gameObject);
			}
		}
	}
	
	public void distractNunsArray(){
		if(nuns.Length != 0){
			for(int i = 0; i < nuns.Length; i++){
				if(!nuns[i].getInvest()){
					nuns[i].setTimeAfterDistraction(waiting_time_after_distraction);
					nuns[i].activateNormalInvestigate(transform.gameObject,distraction_distance_before_stopping);
				}
			}			
		}
		changeParticlesColor();
		playFeedbackSound();
	}	
	
	public void changeParticlesColor(){
		transform.parent.GetComponentInChildren<DistractionParticleController>().changeParticlesColor(waiting_time_after_distraction);
	}
	
	public void playFeedbackSound(){		
		if(audioSource!=null) audioSource.Play();
	}
		
}
