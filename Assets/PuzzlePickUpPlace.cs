using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum KidPuzzleType{Teddy=1, Football=2, Plant=3, Colors=4, None=0}

public class PuzzlePickUpPlace : Action {

	public static KidPuzzleType holdingType=KidPuzzleType.None;
	public KidPuzzleType puzzleType;
	private GUITexture puzzleShower;
	private KidPuzzleController controller;

	void Start () {
		puzzleShower = GameObject.FindGameObjectWithTag ("PuzzleShower").guiTexture;
		controller = transform.root.GetComponent<KidPuzzleController> ();
	}

	public override void execute(){
		Texture auxTexture=null;

		KidPuzzleType auxPuzzleType = KidPuzzleType.None;

		//first leave
		if (puzzleShower.enabled) {

			auxPuzzleType = puzzleType;
			auxTexture=renderer.material.mainTexture;
			renderer.material.mainTexture=puzzleShower.texture;
			renderer.enabled=true;
			puzzleType=holdingType;
			holdingType=KidPuzzleType.None;
		}

		//then pick up
		if (auxTexture != null || !puzzleShower.enabled) {
			//it's a different object

			puzzleShower.enabled = true;

			if (auxTexture != null){
				puzzleShower.texture = auxTexture;
				holdingType=auxPuzzleType;
			} else {
				puzzleShower.texture = renderer.material.mainTexture;
				holdingType=puzzleType;

				renderer.material.mainTexture = null;
				renderer.enabled = false;
				puzzleType=KidPuzzleType.None;
			}



		} else {
			puzzleShower.enabled = false;
		}

		//check if solved
		if (!puzzleShower.enabled) {
			controller.VerifyPuzzle();
		}
	}
	
}
