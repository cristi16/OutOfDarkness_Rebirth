using UnityEngine;
using System.Collections;

public class CameraDisableClipping : MonoBehaviour {

	internal Vector3 hidingOffset;
	internal bool active=false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		camera.nearClipPlane = 0.01f;
	}

	void OnPreCull(){
		//if(active) camera.transform.position += hidingOffset;
	}

	void OnPostRender(){
		//if(active) camera.transform.position -= hidingOffset;
	}

	public void EnableHiding(){
	}

	public void DisableHiding(){
	}

}
