using UnityEngine;
using System.Collections;

public class FallAction : Action {
		
	private GameObject player;
	private Vector3 originalPosition;
	private bool executed=false;
	
	public override void execute(){		
		if(!executed){
			audio.Play();
			GetComponentInChildren<Rigidbody>().AddForce(new Vector3(-10f,-2f,10f),ForceMode.Impulse);
			//GetComponentInChildren<Rigidbody>().AddForce(new Vector3(transform.position.x - playerPos.x,transform.position.y - playerPos.y,transform.position.z - playerPos.z).normalized * 500f);
			executed=true;
		}
	}
		
	
	void Start(){
		player = GameObject.FindGameObjectWithTag("Kid");
		originalPosition = transform.position;
	}
	
	void Update(){		
	}
}
