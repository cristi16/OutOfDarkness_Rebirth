using UnityEngine;
using System.Collections;

public class DrawingAction : ActionOOD {
	public GameObject drawingOne;

	private Color colorToFadeOut;

	private bool running=false;
	private float a=0f;
	public AudioSource audioSource;

	public override void execute(){		
		running = true;	
		colorToFadeOut = drawingOne.renderer.material.color;
		audioSource.Play();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (running) {
			colorToFadeOut.a-=Time.deltaTime*0.5f;
			if(colorToFadeOut.a<=0f){
				colorToFadeOut.a=0f;
				running=false;
			}

			drawingOne.renderer.material.color=colorToFadeOut;
		}
	}
}
