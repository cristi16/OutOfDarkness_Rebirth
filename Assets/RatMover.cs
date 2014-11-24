using UnityEngine;
using System.Collections;

public class RatMover : MonoBehaviour {
	internal bool start=false;
	private float time=0f;
	// Use this for initialization
	void Start () {
	}
	 
	// Update is called once per frame
	void Update () {
		if (start) {
			time+=Time.deltaTime;
			if(time<4.0f){
				Vector3 posx = new Vector3(Random.Range(-0.1f,0.1f),0f,0f);
				transform.Translate((posx-Vector3.forward)*Time.deltaTime*12f);
			} else {
				Destroy (transform.gameObject);
			}
		}
	} 

	public void Move(){
		start = true;
		audio.Play(); 
	}
}
