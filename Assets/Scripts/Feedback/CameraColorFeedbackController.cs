using UnityEngine;
using System.Collections;

public class CameraColorFeedbackController : MonoBehaviour {
	
	public bool investigating=false;
	public bool chasing=false;	
	public float colorIncrement=0.01f;
	public Texture2D investigatingTexture;
	public Texture2D chasingTexture; 
	
	public float lowerBorder=0f;
	public float upperBorder=0.6f;
	
	private bool oldInvestigating=false;
	private bool oldChasing=false;
	private ScreenOverlay screenOverlayShader;
	private bool active=false;
	private float intensity=0f;
		
	private bool goingUp=true;	
	public float minimalChaseTime=2.0f;
	private float time=0f;
	private bool changeToInvestigation=false;
	public GUIText hideText;
	public bool feedbackTextActivated=false;
	
	private bool firstTime=true;
	private bool showText = true;
	
	private AudioSource heartBeat;
	private AudioSource breathing;
	private HidingController hc;
	
	// Use this for initialization
	void Start () {
		screenOverlayShader = GetComponent<ScreenOverlay>();		
		hideText.material.color = new Color(1f,1f,1f,0f);
		hideText.text="Hide!";	
		heartBeat = GameObject.FindGameObjectWithTag("heartBeat").GetComponent<AudioSource>();
		breathing = GameObject.FindGameObjectWithTag("breathing").GetComponent<AudioSource>();
		hc = GameObject.FindGameObjectWithTag("Kid").GetComponentInChildren<HidingController>();
	}
	
	// Update is called once per frame
	void Update () {				
		
		if(chasing){
			time+=Time.deltaTime;
		}
		
		float changeRatio = colorIncrement * Time.deltaTime * 80;
		
		//No changes
		if(investigating==oldInvestigating && chasing==oldChasing){
			//No change			
			if(chasing && investigating && changeToInvestigation && time>=minimalChaseTime){
				screenOverlayShader.texture = investigatingTexture;
				changeToInvestigation=false;
			}
			if(!investigating && !chasing) showText=true;
		} else {
			//Change
			firstTime=true;

			if(chasing){				
				screenOverlayShader.texture = chasingTexture;
				if(!oldChasing){
					time=0f;
				}				
				heartBeat.pitch=1.3f;
				heartBeat.volume = heartBeat.gameObject.GetComponent<MusicManager>().audioVolume;
				heartBeat.Play();
				
				breathing.pitch=1.0f;
				breathing.volume = breathing.gameObject.GetComponent<MusicManager>().audioVolume;
				breathing.Play();
			} else if(investigating){
				if(!oldChasing){
					screenOverlayShader.texture = investigatingTexture;		
				} else {
					if(time>=minimalChaseTime){
						screenOverlayShader.texture = investigatingTexture;		
						changeToInvestigation=false;
					} else {
						changeToInvestigation=true;
					}
				}
				heartBeat.pitch=1.2f;
				heartBeat.volume = heartBeat.gameObject.GetComponent<MusicManager>().audioVolume;
				heartBeat.Play();
				
				breathing.pitch=0.9f;
				breathing.volume = breathing.gameObject.GetComponent<MusicManager>().audioVolume;
				breathing.Play();
			}
			
			oldInvestigating = investigating;
			oldChasing = chasing;
		}						
		
		if(investigating || chasing){
			//change intensity value
			if(intensity<=lowerBorder){
				goingUp=true;
				if(hc.hiding) showText=false;
			} else if(intensity>=upperBorder){
				goingUp=false;
			}
			intensity+=(goingUp?changeRatio:-changeRatio);
			//Double speed if chasing
			if(chasing || changeToInvestigation){
				intensity+=(goingUp?changeRatio:-changeRatio)/2;
			}
			//Turning off feedback
		} else{ 
			if(firstTime){
				firstTime=false;				
				heartBeat.gameObject.GetComponent<MusicManager>().reduceVolumeTemp();
				breathing.gameObject.GetComponent<MusicManager>().reduceVolumeTemp();
			}			
			if(intensity>lowerBorder){
				intensity-=colorIncrement;
				if(intensity<lowerBorder){
					intensity=lowerBorder;
				}				
			}
		}
		
		screenOverlayShader.intensity=intensity;
		if(feedbackTextActivated){
			if(showText){
				hideText.material.color = new Color(1f,1f,1f,intensity*80*3 * Time.deltaTime);
			}
		}
	}
		
}
