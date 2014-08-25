using UnityEngine;
using System.Collections;

public class RatActivator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame 
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.tag=="Kid"){
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Rat")){
				g.GetComponent<RatMover>().Move(); 
			}
			collider.enabled = false;
		}
	}
}
