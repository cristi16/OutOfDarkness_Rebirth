using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectEnabler : MonoBehaviour {
	
	public List<GameObject> gameObjectsToBeEnabled;
	
	private InteractiveTrigger hotSpot;
	private InteractiveCollider interactiveObject;	
	
	void Start () {
		hotSpot = GetComponent<InteractiveTrigger>();
		interactiveObject = GetComponent<InteractiveCollider>();
	}
	
	public void Enable(){
		foreach(GameObject g in gameObjectsToBeEnabled) if(g!=null) g.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		if(hotSpot!=null){
			if(hotSpot.getGui()){	
				Enable();
			}
		}
		
		if(interactiveObject!=null){
			if(interactiveObject.activateHelpCondition()){
				Enable ();
			}
		}		
	}
	
	void OnTriggerEnter(Collider other){
		if(hotSpot==null && interactiveObject==null && other.tag=="Kid"){
			Enable();
		}
	}
}
