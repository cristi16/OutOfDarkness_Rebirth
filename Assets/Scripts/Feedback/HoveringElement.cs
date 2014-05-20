using UnityEngine;
using System.Collections;

public class HoveringElement : MonoBehaviour {
	
	public float hoverAmount = 10f;
	public float speed=10f;
	private bool goingUp=true;
	private Vector3 originalPosition;
	private float sign;	
	
	// Use this for initialization
	void Start () {
		originalPosition = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,gameObject.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if(goingUp){
			sign=1;
		} else {
			sign=-1;
		}		
		gameObject.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+sign*speed*Time.deltaTime,gameObject.transform.position.z);		
		
		if(gameObject.transform.position.y>=originalPosition.y + hoverAmount/2){
			goingUp=false;
			gameObject.transform.position=new Vector3(originalPosition.x,originalPosition.y + hoverAmount/2,originalPosition.z);
		} else if(gameObject.transform.position.y<=originalPosition.y - hoverAmount/2){
			goingUp=true;
			gameObject.transform.position=new Vector3(originalPosition.x,originalPosition.y - hoverAmount/2,originalPosition.z);
		}
		
	}
		
}
