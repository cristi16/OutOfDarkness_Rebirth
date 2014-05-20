using UnityEngine;
using System.Collections;
using System;

public class NoisyObject : MonoBehaviour {
	
	public NunStateMachine[] nuns; 
	private AudioSource audioSource;
	private AudioClip noiseSound;
	private AudioManager am;
	
	// Use this for initialization
	void Start () {
		am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		noiseSound = am.noisyObject;
		audioSource = GetComponent<AudioSource>();
	}
	
	public void OnTriggerEnter(Collider col){
		if(col.CompareTag("Kid")){
			if(!audioSource.isPlaying) audioSource.PlayOneShot(noiseSound);
			if(nuns.Length != 0){
				for(int i = 0; i < nuns.Length; i++){
					if(nuns[i].CurrentStateEqualTo(NunStateMachine.NunStates.Default) ||
						nuns[i].CurrentStateEqualTo(NunStateMachine.NunStates.Investigating)){
						//Debug.Log(nuns[i].name + " heard the noise");
						//nuns[i].activateNormalInvestigate(gameObject.transform,0.5f,true);												
						//nuns[i].activateChasingInvestigate(GameObject.FindGameObjectWithTag("Kid"), 6.0f,true);
						nuns[i].ActivateDistractionInvestigation(transform);
					}
					else
					{
						Debug.Log("else");	
					}
				}
			}
		}
	}
	
		
}
