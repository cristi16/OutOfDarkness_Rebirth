using UnityEngine;
using System.Collections;

public class BlockDoor : MonoBehaviour {
	
	DoorInteraction door;
	
	// Use this for initialization
	void Start () {
		door = transform.parent.GetComponent<DoorInteraction>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void OnTriggerStay(Collider hit){
	
		if(hit.gameObject.tag == "Kid")
		{
			if(door.isClosed && door.state == DoorInteraction.DoorState.Idle)
			{
				door.isUnusable = true;	
				door.initiallyLocked = true;
			}
		}
	}
}
