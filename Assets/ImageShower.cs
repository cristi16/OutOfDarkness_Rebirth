using UnityEngine;
using System.Collections;

public class ImageShower : MonoBehaviour {
	
	private bool gui = false;
	public float image_dimension = 0.8f;
	public Texture2D image;
	private float timer;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider col){
		if(col.CompareTag("Kid") && SwitchMechanic_Script.getKidControl()){
			if( Input.GetButton("Interaction") && timer < Time.time && !gui){
				
				gui = true;
				timer = Time.time + 0.3f;								
			}
		}
	}
	
	void OnTriggerExit(Collider col){
		if(col.CompareTag("Kid") || (col.CompareTag("Ghost") && !SwitchMechanic_Script.getKidControl())){ //If the kid exit or the ghost exit while the player is controlling it
			gui = false;
		}
	
	}
	
	void OnGUI(){
		if(!gui)
			return;
				
		GUI.DrawTexture(new Rect (Screen.width/4 ,50, Screen.width*0.5f, Screen.height*0.8f), image);
		
		if(Input.GetButton("Interaction") && timer < Time.time){
			gui=false;
			timer = Time.time +0.3f;
		}
	
	}
}
