using UnityEngine;
using System.Collections;

public class OVRCalibrator : MonoBehaviour {

	private float offsetX=0f;

	// Use this for initialization
	void Start () {
	}

	void SetAspectRatio(){
		//Screen.SetResolution(LevelState.getInstance().resolution,LevelState.getInstance().resolution/2,false);

		float oldAspect = Screen.width / Screen.height;
		foreach (Camera c in GetComponentsInChildren<Camera>()) {
			//c.fieldOfView=150f;		
			c.aspect=oldAspect/2;
		}

		offsetX -= 0.001f;

		Rect aux = GetComponentsInChildren<Camera> () [0].rect;
		aux.x -= offsetX;
		GetComponentsInChildren<Camera> () [0].rect = aux;
		
		Rect aux2 = GetComponentsInChildren<Camera> () [1].rect;
		aux2.x += offsetX;
		GetComponentsInChildren<Camera> () [1].rect = aux2;

	}

	// Update is called once per frame
	void Update () {

		
		//Debug
		if (Input.GetKey (KeyCode.Z)) {
			//con.IPD+=0.01f;

			Invoke ("SetAspectRatio",0.01f);

		}
		
		if (Input.GetKey (KeyCode.X)) {
			//con.IPD-=0.01f;

			Invoke ("SetAspectRatio",0.01f);
		}


	}

	void OnGUI(){
	}
}
