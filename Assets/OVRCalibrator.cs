using UnityEngine;
using System.Collections;

public class OVRCalibrator : MonoBehaviour {
	private OVRCameraController con;

	// Use this for initialization
	void Start () {
		con = GetComponent<OVRCameraController> ();
	}

	void SetAspectRatio(){
		Screen.SetResolution(Screen.width,Screen.width/2,false);

		foreach (Camera c in GetComponentsInChildren<Camera>()) {
			//c.fieldOfView=150f;
			c.aspect=4f/3f;
		}
	}

	// Update is called once per frame
	void Update () {

		
		//Debug
		if (Input.GetKey (KeyCode.O)) {
			con.IPD+=0.05f;

			Invoke ("SetAspectRatio",0.01f);

		}
		
		if (Input.GetKey (KeyCode.P)) {
			con.IPD-=0.05f;

			Invoke ("SetAspectRatio",0.01f);
		}


	}

	void OnGUI(){
	}
}
