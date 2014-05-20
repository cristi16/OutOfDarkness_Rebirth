using UnityEngine;
using System.Collections;

public class ScareController : MonoBehaviour {
	private bool scared=false;
	private AudioSource scaredSound;
	private float afterScareTime=4.5f;
	
	private bool fadeOut=false;
	
	// Use this for initialization
	void Start () {
		scaredSound = GameObject.FindGameObjectWithTag("Heartbeat").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {	
		if(fadeOut){
			scaredSound.volume-=Time.deltaTime/afterScareTime;
			if(scaredSound.volume<=0) fadeOut=false;
		}
	}
	
	public void Scared(){
		scared=true;
		GetComponent<NoiseEffect>().enabled=true;
		scaredSound.volume=1f;
		fadeOut=false;
		scaredSound.Play();
	}
	
	public void NotScared(){	
		Invoke("DeactivateScare",afterScareTime);
		fadeOut=true;
	}
	
	private void DeactivateScare(){
		scared=false;		
		GetComponent<NoiseEffect>().enabled=false;
		scaredSound.Pause();
	}
}
