using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NunChaseInvestigateManager : MonoBehaviour {
	
	public HashSet<AI> nunsInvestigating;
	public HashSet<AI> nunsChasing;	
	private CameraColorFeedbackController cameraController;
	private bool someone_investigating=false;
	private bool someone_chasing=false;
	private AudioSource audio;
	public AudioClip chase;
	public AudioClip investigate;
	private MusicManager music;
	
	// Use this for initialization
	void Start () {
		audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
		nunsInvestigating = new HashSet<AI>();
		nunsChasing = new HashSet<AI>();
		cameraController=Camera.main.GetComponent<CameraColorFeedbackController>();	
		music = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>();
	}
	
	public void AddNun(AI nun, bool chasing){
		if(chasing){
			if(!containsChasingNun(nun))  
				nunsChasing.Add(nun);
		} else {
			if(!containsInvestigatingNun(nun))
				nunsInvestigating.Add(nun);
		}
		UpdateCameraFeedbackState();
	}
	
	private void PlayAudio(AudioClip audioClip){
		music.reduceVolumeTemp();		
		this.audio.PlayOneShot(audioClip);
	}
	
	public void RemoveNun(AI nun, bool chasing){
		if(chasing){
			nunsChasing.Remove(nun);
		} else {
			nunsInvestigating.Remove(nun);
		}
		UpdateCameraFeedbackState();
	}
	
	public void RemoveNun(AI nun){
		if(nunsChasing.Contains(nun)) nunsChasing.Remove(nun);
		if(nunsInvestigating.Contains(nun)) nunsInvestigating.Remove(nun);
		UpdateCameraFeedbackState();
	}
	
	void UpdateCameraFeedbackState(){
		if(nunsChasing.Count>0){
			if (!someone_chasing) {
				someone_chasing=true;
				PlayAudio(chase);
			}
			cameraController.chasing=true;
		} else {
			someone_chasing=false;
			cameraController.chasing=false;
		}
		
		if(nunsInvestigating.Count>0){
			if (!someone_investigating) {
				someone_investigating=true;
				PlayAudio(investigate);
			}
			cameraController.investigating=true;
		} else {
			someone_investigating=false;
			cameraController.investigating=false;
		}
	}	
	
	private bool containsInvestigatingNun(AI temp){
		if(nunsInvestigating.Contains(temp))
			return true;
		else
			return false;
	}
	
	private bool containsChasingNun(AI temp){
		if(nunsChasing.Contains(temp))
			return true;
		else
			return false;
	}
	
	// Update is called once per frame
	void Update () {		
	}
}
