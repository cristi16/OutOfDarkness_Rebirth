using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectDestroyer : MonoBehaviour {
	
	public List<GameObject> gameObjectsToBeDestroyed;
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;
	private bool destroyed=false;
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	public void Destroy(){
		foreach(GameObject g in gameObjectsToBeDestroyed) Destroy (g);	
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui() || destroyed){	
				Destroy ();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition() || destroyed){
				Destroy ();				
			}
		}				
	}
	
	void OnTriggerEnter(Collider other){
		if(hotSpot==null && interactiveObject==null && other.tag=="Kid"){
			Destroy();
		}
	}
}
