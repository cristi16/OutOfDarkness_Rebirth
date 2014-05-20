using UnityEngine;
using System.Collections;

public class DoorDodging : MonoBehaviour {
 	
	private DoorInteraction door;
	private bool wasHere = false;
	private Vector3 destination;
	private Vector3 velocity = Vector3.zero;
	private Transform _transform;
	private TP_Controller controller;
	
	void Start () {
		_transform = transform.parent;
		controller = transform.parent.GetComponent<TP_Controller>();
	}
	
	void Update()
	{	
		if(door != null)
		{
			if(wasHere == false)
			{
				destination = door.GetSafePosition();
				Debug.DrawLine(destination, transform.position, Color.green);
				wasHere = true;
				controller.hasControl = false;
			}
			
			// Move to safe point if a door triggered our collider
			_transform.position = Vector3.SmoothDamp(_transform.position, 
			new Vector3(destination.x, _transform.position.y, destination.z), ref velocity, door.smoothFactor, door.maxSpeed);
			
			if(door.state == DoorInteraction.DoorState.Idle)
			{
				door = null;
				wasHere = false;
				controller.hasControl = true;
			}
		}
	}
 
 	void OnTriggerEnter(Collider hit)
	{	
		if(hit.gameObject.tag == "Door")
		{	
			door = hit.transform.parent.GetComponentInChildren<DoorInteraction>();
		}
	}	
}
