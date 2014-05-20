using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectiblesManager_Script : MonoBehaviour {
	
	//public string text = "Notes";
	private int total_collectibles = 0;
	private int collected = 0;
	public GUIText notesText;
	public GUITexture collectiblesTexture;
	internal bool showTexture=false;
	
	void OnGUI(){
		if(!showTexture){			
			collectiblesTexture.texture=null;
		}
	}
	
	void Update(){
		notesText.text = "Notes: "+collected +"/"+total_collectibles;
		showTexture=false;
	}
	
	public void countCollectibles(){
		total_collectibles++;
	}
	
	public void addCollectible(){
		collected++;
	}
}
