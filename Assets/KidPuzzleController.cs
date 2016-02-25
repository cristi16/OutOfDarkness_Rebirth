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
		puzzleShower = GameObject.FindGameObjectWithTag ("PuzzleShower").GetComponent<GUITexture>();
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
			else if(GetComponent<AudioSource>()!=null) GetComponent<AudioSource>().Play ();
		}
	}

	public void SolvePuzzle(){		
		teddy.GetComponent<Collider>().enabled = false;
		plant.GetComponent<Collider>().enabled = false;
		football.GetComponent<Collider>().enabled = false;
		colors.GetComponent<Collider>().enabled = false;

		LevelState.getInstance().solvedKidPuzzle=true;
		bookcaseAfterPuzzle.SetActive(true);
		bookcaseBeforePuzzle.SetActive(false);
	}

	void OnTriggerExit(){
		if (puzzleShower.enabled) {
			puzzleShower.enabled=false;

			foreach(PuzzlePickUpPlace p in GetComponentsInChildren<PuzzlePickUpPlace>()){
				if(!p.GetComponent<Renderer>().enabled){
					p.GetComponent<Renderer>().enabled=true;
					p.GetComponent<Renderer>().material.mainTexture=puzzleShower.GetComponent<GUITexture>().texture;
					p.puzzleType=PuzzlePickUpPlace.holdingType;
					puzzleShower.GetComponent<GUITexture>().texture=null;
					PuzzlePickUpPlace.holdingType=KidPuzzleType.None;
				}

			}
		}
	}
}
