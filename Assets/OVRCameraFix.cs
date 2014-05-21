using UnityEngine;
using System.Collections;

public class OVRCameraFix : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(0.2f);
		GetComponent<OVRCameraController>().SetIPD (0f);
		yield return new WaitForSeconds(0.2f);
		foreach (Camera c in GetComponentsInChildren<Camera>()) {
			c.fieldOfView=150f;
			c.aspect=4f/3f;
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
