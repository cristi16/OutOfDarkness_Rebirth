using UnityEngine;
using System.Collections;

public class RemoveHoveringText : MonoBehaviour {
	
	public bool requireKey=false;
	private bool checkPoint=false;
	public bool requireInteraction=true;
	
	// Use this for initialization
	void Start () {
		if(GetComponent<Checkpoint_Script>()!=null) checkPoint=true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider col){
		
		if(col.CompareTag("Kid") || col.CompareTag("Ghost")){
			if( Input.GetButton("Interaction") || !requireInteraction )
			{	
				HoveringElement element = transform.parent.GetComponentInChildren<HoveringElement>();
				
				if(checkPoint) element = GetComponentInChildren<HoveringElement>();
				//Remove text tags
				if(element!=null && (!requireKey || transform.parent.GetComponentInChildren<DoorInteraction>().hasKey)) GameObject.Destroy(element.gameObject);
			}
		}
	}	
}
