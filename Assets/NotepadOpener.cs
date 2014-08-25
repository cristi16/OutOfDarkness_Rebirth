using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotepadOpener : MonoBehaviour {
	private GameObject notepad;
	private TP_Controller player;
	public List<Texture2D> notepads;

	// Use this for initialization
	void Start () {
		notepad = GameObject.FindGameObjectWithTag("Notepad");
		player = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!LevelState.getInstance ().notepadActivated)
						return;
		if (player.hasControl && Input.GetButton ("Notepad") && !notepad.renderer.enabled) {
			notepad.renderer.material.mainTexture=notepads[LevelState.getInstance().objectives];
			notepad.renderer.enabled=true;
			player.GetComponentInChildren<TP_Controller>().removeControl();
		} else if(!Input.GetButton ("Notepad") && notepad.renderer.enabled) {
			notepad.renderer.enabled=false;
			player.GetComponentInChildren<TP_Controller>().returnControl();
		}
	}
}
