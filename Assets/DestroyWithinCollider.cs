using UnityEngine;
using System.Collections;

public class DestroyWithinCollider : MonoBehaviour {

	private float time=2f;
	private bool destroy=false;
	private GameObject gameObjectToDestroy;
	private AudioSource audio;
	private float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (destroy) {
			time-=Time.deltaTime;

			if(time<=0){
				destroy=false;
				Destroy();
			} else {
				audio.volume-=speed*Time.deltaTime;
			}

		}		
	}

	void Destroy(){
		Destroy (gameObjectToDestroy);
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "Nun" && !destroy) {
			destroy = true;
			gameObjectToDestroy = col.gameObject;
			audio=gameObjectToDestroy.GetComponentInChildren<NoiseTrigger>().audio;
			speed = audio.volume/time;
		}
	}
}
