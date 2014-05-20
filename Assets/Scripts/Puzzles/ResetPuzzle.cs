using UnityEngine;
using System.Collections;

public class ResetPuzzle : MonoBehaviour {
	
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
			collider.enabled = true;
			renderer.enabled = true;
		}
		else
		{
			collider.enabled = false;
			renderer.enabled = false;
		}
	}
	
	void OnMouseUp()
	{
		if(puzzleController.canExit)
			puzzleController.ResetPuzzle();	
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
