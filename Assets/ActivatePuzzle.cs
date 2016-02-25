using UnityEngine;
using System.Collections;

public class ActivatePuzzle : ActionOOD {
	
	public GameObject puzzle;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void execute(){
		if(!puzzle.GetComponent<PuzzleController>().isActivated && (Time.time-puzzle.GetComponent<PuzzleController>().timeLastDisable)>2.0f){
			if(puzzle.GetComponent<PuzzleController>().canExit){
				puzzle.SetActive(true);
				Invoke ("puzzleActivation",0.1f);
			} else {
				transform.GetChild(0).GetComponent<TextTrigger>().ShowMessages();
			}			
		}		
	}
	
	void puzzleActivation(){
		puzzle.GetComponent<PuzzleController>().CheckPuzzleActivation();
	}
}
