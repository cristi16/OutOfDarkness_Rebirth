using UnityEngine;
using System.Collections;


public class KidPuzzleController : MonoBehaviour {

	private GUITexture puzzleShower;

	public PuzzlePickUpPlace teddy;
	public PuzzlePickUpPlace football;
	public PuzzlePickUpPlace plant;
	public PuzzlePickUpPlace colors;
	public GameObject bookcaseBeforePuzzle;
	public GameObject bookcaseAfterPuzzle;
	private TextTrigger puzzleSolvedTrigger;
	public AudioSource audioSource;
	
	void Start () {
		puzzleShower = GameObject.FindGameObjectWithTag ("PuzzleShower").guiTexture;
		puzzleSolvedTrigger = GetComponent<TextTrigger> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void VerifyPuzzle(){
		if (teddy.puzzleType == KidPuzzleType.Teddy && football.puzzleType == KidPuzzleType.Football && 
		    plant.puzzleType == KidPuzzleType.Plant && colors.puzzleType == KidPuzzleType.Colors) {
		    SolvePuzzle();
			Debug.Log("Solved");
			puzzleSolvedTrigger.ActivateTextTrigger();
			if(audioSource!=null) audioSource.Play();
			else if(audio!=null) audio.Play ();
		}
	}

	public void SolvePuzzle(){		
		teddy.collider.enabled = false;
		plant.collider.enabled = false;
		football.collider.enabled = false;
		colors.collider.enabled = false;

		LevelState.getInstance().solvedKidPuzzle=true;
		bookcaseAfterPuzzle.SetActive(true);
		bookcaseBeforePuzzle.SetActive(false);
	}

	void OnTriggerExit(){
		if (puzzleShower.enabled) {
			puzzleShower.enabled=false;

			foreach(PuzzlePickUpPlace p in GetComponentsInChildren<PuzzlePickUpPlace>()){
				if(!p.renderer.enabled){
					p.renderer.enabled=true;
					p.renderer.material.mainTexture=puzzleShower.guiTexture.texture;
					p.puzzleType=PuzzlePickUpPlace.holdingType;
					puzzleShower.guiTexture.texture=null;
					PuzzlePickUpPlace.holdingType=KidPuzzleType.None;
				}

			}
		}
	}
}
