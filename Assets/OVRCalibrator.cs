using UnityEngine;
using System.Collections;

public class OVRCalibrator : MonoBehaviour {
	private OVRCameraController con;

	// Use this for initialization
	void Start () {
		con = GetComponent<OVRCameraController> ();
	}
	
	// Update is called once per frame
	void Update () {

		
		//Debug
		if (Input.GetKey (KeyCode.Z)) {
			con.IPD+=0.05f;

		}
		
		if (Input.GetKey (KeyCode.X)) {
			con.IPD-=0.05f;
		}
	}

	void OnGUI(){
	}
}
