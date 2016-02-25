using UnityEngine;
using System.Collections;

public class ExitPuzzle : MonoBehaviour {
	
	private PuzzleController puzzleController;
	public float onHoverScaleFactor = 0.05f;
	
	// Use this for initialization
	void Start () {
		puzzleController = transform.parent.GetComponent<PuzzleController>();
	}
	
	void Update()
	{
		if(puzzleController.isActivated)
		{
			GetComponent<Collider>().enabled = true;
			GetComponent<Renderer>().enabled = true;
		}
		else
		{
			GetComponent<Collider>().enabled = false;
			GetComponent<Renderer>().enabled = false;
		}
	}
	
	void OnMouseUp()
	{
		if(puzzleController.canExit)
			puzzleController.DeactivatePuzzle();	
	}
	
	void OnMouseEnter()
	{
		transform.localScale += new Vector3(onHoverScaleFactor, onHoverScaleFactor, onHoverScaleFactor);
	}
	
	void OnMouseExit()
	{
		transform.localScale -= new Vector3(onHoverScaleFactor, onHoverScaleFactor, onHoverScaleFactor);
	}
}
