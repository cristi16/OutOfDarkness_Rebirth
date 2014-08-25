using UnityEngine;
using System.Collections;

public class ActivateDeactivateNurseryRhymeObjectives : MonoBehaviour {
	private bool firstTime=true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (LevelState.getInstance().rhymesFound==4 && firstTime) {
			firstTime=false;
			collider.enabled=true;
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("NurseryRhymeObjectives")){
				g.collider.enabled=false;
			}
		}
	}
}
