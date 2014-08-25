using UnityEngine;
using System.Collections;

public class TitleShower : MonoBehaviour {
	
	private bool gui = false;
	
	private float timer=0f;
	public float timeToShow=3f;
	public float timeToAppearDisappear=1f;
	public GUITexture title;
	private bool started=false;

	private bool fadingIn=false;
	private bool fadingOut=false;
	private bool waiting=false;
	private Color color;
	public int chapter=-1;
	
	// Use this for initialization
	void Start () {
		color = title.color;
		color.a = 0f;
		title.color = color;
	}		

	void Update(){
		if (!started)
						return;

		//if(Input.GetButton("Interaction")) fadingOut=true;

		if (fadingIn) {
			color.a+=(0.5f/timeToAppearDisappear) * Time.deltaTime;
			if(color.a>0.5f){
				color.a=0.5f;
				fadingIn=false;
				waiting=true;
				timer = timeToShow;
			}
		}

		if (waiting) {
			timer-=Time.deltaTime;
			if(timer<0){
				waiting=false;
				fadingOut=true;
			}
		}

		if (fadingOut) {
			color.a-=(0.5f/timeToAppearDisappear) * Time.deltaTime;
			if(color.a<0){
				color.a=0;
				fadingOut=false;
				enabled=false;
			}
		}
		title.color = color;
	}

	void FadeIn(){
		if (chapter==-1 || LevelState.getInstance ().chapter < chapter) {		
			started = true;
			fadingIn = true;
			if(chapter!=-1) LevelState.getInstance ().chapter = chapter;
			else{
				LevelState.getInstance().objectives++;
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			if(!started) FadeIn();
		}
	}
	
	void OnTriggerExit(Collider col){
	}
	
	void OnGUI(){
		if(!started)
			return;
			
	}

	
}
