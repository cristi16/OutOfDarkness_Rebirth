using UnityEngine;
using System.Collections;

public class showCommands : MonoBehaviour {
	
	public Texture2D image;
	private bool gui = false;
	public float duration = 11f;
	public float image_dimension = 200f;
	private float alpha = 1;
	private Color guiColor;
	public float x = 50;
	private float time=0f;
	private float timeToDisappear=3.0f;
	public bool showUntilTriggerExit=false;
	public bool centered=true;
	public bool pause=false;
	
	public TutorialStartCamera tut;
	
	void Start(){
		guiColor = GUI.color;
		guiColor.a = 1;
	
	}
	
	void Update(){
		if(gui){
			time+=Time.deltaTime;
			
			if(time>duration && !showUntilTriggerExit){
				alpha -= Time.deltaTime/timeToDisappear;
			}							
			
			if(alpha <= 0 ) Destroy(transform.gameObject);
		}
	}
	
	// Update is called once per frame
	void OnTriggerStay(Collider collider){
		if(tut == null){
			if(collider.CompareTag("Kid") && !gui){
				gui=true;
			}
		}
	}
	
	void OnTriggerExit(Collider collider){
		
		if(collider.CompareTag("Kid")){
			if(showUntilTriggerExit){
				showUntilTriggerExit=false;
			}
			//Destroy(transform.gameObject);
		}
	}
	
	void OnGUI(){	
		if(!gui || pause)
			return;
		
		guiColor.a = alpha;
		GUI.color = guiColor;
		if(centered){
			GUI.Label(new Rect ((Screen.width-image_dimension)/2,(Screen.height-image_dimension)/2 + x, image_dimension, image_dimension), image);
		} else {
			GUI.Label(new Rect (Screen.width - image_dimension - 10,Screen.height * 0.7f, image_dimension, image_dimension), image);
		}
	}
}
