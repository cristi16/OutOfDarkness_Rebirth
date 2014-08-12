using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent (typeof (AudioSource))]
public class NoiseTrigger : MonoBehaviour {

	private SneakWalkRunController player_sneak;
	private AudioManager audioManager;
	private AudioSource source;
	private AudioClip noiseAlertClip;
	private NunStateMachine nun_ai;
	private int layerMask;
	private RaycastHit hitInfo;
	
	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindGameObjectWithTag("Kid");
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		noiseAlertClip = audioManager.nunNoiseAlert;
		source = gameObject.GetComponent<AudioSource>();
		source.minDistance = gameObject.GetComponent<SphereCollider>().radius;
		
		// Take into account only the following layers
		layerMask = 1 << LayerMask.NameToLayer("Wall");
		layerMask += 1 << LayerMask.NameToLayer("Doors");
		layerMask += 1 << LayerMask.NameToLayer("Player");
		layerMask += 1 << LayerMask.NameToLayer("GhostCollider");
		
		nun_ai = transform.parent.GetComponent<NunStateMachine>();
		
		if(player == null){
			Debug.LogError("Error in the initialization of NoiseTrigger script");
			return;
		}	

		player_sneak = player.GetComponent<SneakWalkRunController>();
	}		
	 
	void OnTriggerStay(Collider collider){
		// if the kid enters the trigger radius && the kid is not sneaking && the nun is not investigating or chasing
		 
		if(collider.CompareTag("Kid") && nun_ai.CurrentStateEqualTo(NunStateMachine.NunStates.Default)
			&& !player_sneak.getSneak())
		{
			if(Physics.Raycast(transform.position, collider.transform.position - transform.position , 
				out hitInfo, Mathf.Infinity, layerMask))
			{
				if(LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "Player")
				{
					if(source.isPlaying == false)
					{
						source.clip = noiseAlertClip;
						source.Play();
					}
				}
			}
		}
	}
}
