using UnityEngine;
using System.Collections;

public class LookingAt : MonoBehaviour {
	
	public Transform lookAtTransform;
	// Use this for initialization
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(new Vector3(lookAtTransform.position.x,transform.position.y,lookAtTransform.position.z));
	}
}
