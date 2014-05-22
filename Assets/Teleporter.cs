using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public Transform place;
	private bool firstTime=true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (firstTime) {
			transform.position = place.position;
			firstTime=false;
		}
	}
}
