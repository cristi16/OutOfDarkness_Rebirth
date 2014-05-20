using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour {
	
	public AudioClip audioClip;
	public bool multipleUse=false;
	private bool activated=false;
	private AudioManager am;	
	public float timeToActivate=0f;
	private AudioSource audioSource;
	
	private float time=0f;
	private bool delayedActivation=false;
	
	// Use this for initialization
	void Start () {
		am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {	
		if(delayedActivation){
			time+=Time.deltaTime;
			if(time>timeToActivate){
				delayedActivation=false;
				ActivateTrigger();
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(!activated && other.tag=="Kid" && !delayedActivation){	
			if(timeToActivate>0){
				//delayed activation
				delayedActivation=true;
			} else {
				ActivateTrigger();
			}
		}
	}
	
	void ActivateTrigger(){
		audioSource.PlayOneShot(audioClip);
			if(!multipleUse)
				activated=true;
	}
}
