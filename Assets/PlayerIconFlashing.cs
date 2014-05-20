using UnityEngine;
using System.Collections;

public class PlayerIconFlashing : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float scale = (Mathf.Sin(Time.time * 2f) + 1f)/4f + 1f;
		transform.localScale = new Vector3(scale, scale, scale);
	}
}
