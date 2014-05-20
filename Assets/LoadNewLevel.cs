using UnityEngine;
using System.Collections;

public class LoadNewLevel : MonoBehaviour {

	public int nextLevel=1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			LevelState.mainMenuOrder=false;
			MenuManager.newScene=true;
			LevelState.getInstance().NewScene();
			Application.LoadLevel(nextLevel);
		}		
	}
}
