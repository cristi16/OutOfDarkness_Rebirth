using UnityEngine;
using System.Collections;

public class ShowIfOculus : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (!TP_Motor.oculusRift)
						gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
