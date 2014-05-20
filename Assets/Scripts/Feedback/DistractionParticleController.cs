using UnityEngine;
using System.Collections;

public class DistractionParticleController : MonoBehaviour {
	
	private ParticleSystem particles;		
	private float time;
	private float timeToWait;
	private bool colorChanged=false;
	private Light candleLight;
	
	// Use this for initialization
	void Start () {
		particles = GetComponent<ParticleSystem>();
		candleLight = gameObject.GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		if(colorChanged){
			if(time<timeToWait){
				time+=Time.deltaTime;
			} else {
				colorChanged=false;
				particles.startColor=Color.white;
				time=0f;
				
				foreach(ParticleEmitter e in gameObject.GetComponentsInChildren<ParticleEmitter>()){
					e.emit=true;
				candleLight.enabled = true;
					//particlesToDeactivate.SetActive(true);
				}
			}
		}		
	}
	
	public void changeParticlesColor(float timeToWait){
		this.timeToWait=timeToWait;
		particles.startColor=Color.blue;
		colorChanged=true;
		
		foreach(ParticleEmitter e in gameObject.GetComponentsInChildren<ParticleEmitter>()){
			e.emit=false;
		candleLight.enabled = false;	
			//particlesToDeactivate.SetActive(false);
		}
	}
	
}
