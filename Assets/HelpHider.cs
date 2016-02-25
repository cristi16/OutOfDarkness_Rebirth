using UnityEngine;
using System.Collections;

public class HelpHider : MonoBehaviour {
	private bool hiding=false;
	private HelpManager helpManager;

	// Use this for initialization
	void Start () {
		helpManager = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<HelpManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<GUITexture>().color.a>0 && Input.GetButton ("Notepad")) {
			helpManager.hideHelp();
			hiding=true;
		} else if (hiding && GetComponent<GUITexture>().color.a>0 && !Input.GetButton ("Notepad")) {
			helpManager.showHelp();
			hiding=false;
		}
	}
}
