using UnityEngine;
using System.Collections;

public class DestroyAfterSeconds : MonoBehaviour {

	public float timeToDestroy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (timeToDestroy>0f) {
			timeToDestroy-=Time.deltaTime;
		}

		if (timeToDestroy <= 0f) {
			Destroy(this.gameObject);
		}
	}
}
