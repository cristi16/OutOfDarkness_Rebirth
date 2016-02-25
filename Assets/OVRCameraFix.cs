using UnityEngine;
using System.Collections;

public class OVRCameraFix : MonoBehaviour {

	private float resolution=1f/1f;

	// Use this for initialization
	IEnumerator Start () {
		Screen.SetResolution(1024,512,true);

		yield return new WaitForSeconds(0.2f);
		GetComponent<OVRCameraRig>().IPD = 0f;

		yield return new WaitForSeconds(0.2f);
		foreach (Camera c in GetComponentsInChildren<Camera>()) {
			//c.fieldOfView=150f;
			//c.aspect=4f/3f;
		}
	}

	// Update is called once per frame
	void Update () {
	}

}
