using UnityEngine;
using System.Collections;

public class FlashlightOff : MonoBehaviour {

	GameObject turnOffLight = null;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider hit){
		
		if (hit.gameObject.tag == "Kid") {	
			GameObject[] lights = GameObject.FindGameObjectsWithTag("Flashlight");
			turnOffLight = lights[0];

			foreach(GameObject light in lights){
				if(light.GetComponent<AudioSource>()!=null){
					turnOffLight=light;
				}
			}
			turnOffLight.SetActive(false);
		}
	}

	void OnTriggerExit(Collider hit){
		if (hit.gameObject.tag == "Kid") {	
			if(turnOffLight!=null) turnOffLight.SetActive(true);
		}
	}

}
