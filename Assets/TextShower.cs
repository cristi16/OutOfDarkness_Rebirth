using UnityEngine;
using System.Collections;

public class TextShower : MonoBehaviour {
	
	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
		if(!LevelState.getInstance().usingMainMenu){
			GetComponent<GUIText>().enabled=true;
		}
	}
}
