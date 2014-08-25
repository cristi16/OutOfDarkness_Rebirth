using UnityEngine;
using System.Collections;

public class NotepadActivator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			LevelState.getInstance().notepadActivated=true;
			this.enabled=false;
		}
	}
}
