using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {
	
	private AudioSource audio;
	private bool reduceAudio=false;
	private bool augmentAudio=false;
	private float interval;
	private float time=0f;
	public float timeToChangeAudio=3.0f;
	public float audioVolume = 0.25f;
	public bool restoreMusicAutomatically=false;
	public bool beginInSilence=false;
	public bool playAtBeginning=false;
	public bool loadFileFromWWW=false;
	
	public string musicFileURL;
	public bool soundIn3D=false;		
	public bool fadeOutAndStop=false;
	
	private bool startedPlayer=false;
	
	private static Dictionary<string,AudioClip> musicFiles = new Dictionary<string, AudioClip>();
	
	public void reduceVolumeTemp(){	
		reduceAudio=true;
		time=0f;
		augmentAudio=false;
	}
	
	public void augmentVolume(){	
		reduceAudio=false;
		time=0f;
		augmentAudio=true;
	}
	
	IEnumerator loadMusicFile(){
		WWW www = new WWW(musicFileURL);  // start a download of the given URL						
		
		while (www.audioClip.isReadyToPlay == false){
	        yield return www;
	    }
	 
	    audio.clip = www.GetAudioClip(soundIn3D,false,AudioType.OGGVORBIS);
		
		musicFiles.Add(gameObject.name,audio.clip);
		
		startedPlayer=true;
		if(playAtBeginning) audio.Play();
	}
	
	// Use this for initialization
	void Start () {							
		
		audio = GetComponent<AudioSource>();		
		
		if(musicFileURL!="" && !musicFiles.ContainsKey(gameObject.name) && loadFileFromWWW){
			StartCoroutine("loadMusicFile");			
			
		} else {
			if(musicFiles.ContainsKey(gameObject.name)){
				audio.clip = musicFiles[gameObject.name];
			}
			startedPlayer=true;
			if(playAtBeginning) audio.Play();
		}
		
		if(beginInSilence){
			audio.volume=0f;
		} else {
			audio.volume=audioVolume;
		}
		interval = audioVolume/timeToChangeAudio;		
	}
	
	// Update is called once per frame
	void Update () {
		if(!startedPlayer) return;
		if(reduceAudio){
			time+=Time.deltaTime;
			audio.volume-=Time.deltaTime*interval;
			if(audio.volume<0) audio.volume=0;
			
			if(time>timeToChangeAudio){
				reduceAudio=false;
				if(restoreMusicAutomatically) augmentAudio=true;
				if(fadeOutAndStop) audio.Stop();
				time=0f;
			}
		} else if(augmentAudio){
			audio.volume+=Time.deltaTime*interval;
			if(audio.volume>audioVolume) audio.volume=audioVolume;
			
			if(time>timeToChangeAudio){			
				augmentAudio=false;
				time=0f;
			}
		}
	}
}
