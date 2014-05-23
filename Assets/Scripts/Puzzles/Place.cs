using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Place : MonoBehaviour {
	
	public PlaceableObject placedObject;
	public bool isDefaultPlace;
	public bool isEmptyPlace;
	internal PlaceableObject defaultObject;
	private PuzzleController puzzleController;
	
	private Shader defaultShader;
    private Shader pickedItemShader;
	private Shader hoverShader;
	
	internal ObjectState[] solutions;
	
	// Use this for initialization
	void Start () {
		puzzleController = gameObject.transform.parent.GetComponent<PuzzleController>();
		defaultObject = placedObject;
		if(!isDefaultPlace && !isEmptyPlace)
		{
			solutions = transform.parent.FindChild("Solutions").
				FindChild(gameObject.name).GetComponentsInChildren<ObjectState>();
		}
		defaultShader = Shader.Find("Custom/AlphaSelfIllum");
		pickedItemShader = Shader.Find("Transparent/Diffuse");
		hoverShader = Shader.Find("Custom/AlphaSelfIllum");
	}
	
	// Update is called once per frame
	void Update () {
		Camera.main.transform.root.GetComponentInChildren<TP_Motor> ().RayCastForColliders ();
	}
	
	void OnMouseDown()
	{
		if(puzzleController.selectedObject == null) // if we are not holding anything
		{
			if(placedObject != null) // pick it
			{
				puzzleController.selectedObject = placedObject;
				placedObject.selected = true;
				placedObject.renderer.material.shader = pickedItemShader;
				placedObject = null;
			}
			else
			{
				// there is nothing there	
			}
		}
		else // if an object is already held
		{
			if(placedObject == null) // place it
			{
				placedObject = puzzleController.selectedObject;
				placedObject.transform.position = transform.position;
				placedObject.selected = false;
				placedObject.renderer.material.shader = defaultShader;
				puzzleController.selectedObject = null;
				// if it's a mixing puzzle and not a default place, we disable the item renderer and place collider
				if(puzzleController.puzzleType == PuzzleType.Mixing && !isDefaultPlace) 
				{
					placedObject.renderer.enabled = false;
					this.collider.enabled = false;
				}
				
				// check if we solved the puzzle
				puzzleController.CheckForSolution();
			}
			else if(!isDefaultPlace)// swap it (!!not able to swap between default places)
			{
				PlaceableObject aux;
				placedObject.renderer.material.shader = pickedItemShader;
				aux = placedObject;
				placedObject = puzzleController.selectedObject;
				placedObject.transform.position = transform.position;
				placedObject.selected = false;
				placedObject.renderer.material.shader = defaultShader;
				puzzleController.selectedObject = aux;
				puzzleController.selectedObject.selected = true;
			}
		}
	}
	
	void OnMouseEnter()
	{
//		if(placedObject != null)
//			Debug.Log("Entered :" + placedObject.name);	
	}
	
	void OnMouseOver()
	{
		if(placedObject != null)
		{
			placedObject.renderer.material.shader = hoverShader;
			if(puzzleController.showHints)
				puzzleController.hintsText.text = placedObject.hintText;
		}
		
		if(puzzleController.selectedObject == null && Input.GetMouseButtonDown(1))
		{
			placedObject.PerformRightClick();
			puzzleController.CheckForSolution();
		}
	}
	
	void OnMouseExit()
	{
		if(placedObject != null)
		{
			placedObject.renderer.material.shader = defaultShader;
			
			if(puzzleController.showHints)
				puzzleController.hintsText.text = "";
		}
	}
	
	public bool[] GetSolutionEvaluation()
	{
		bool[] results = new bool[solutions.Length];
		for(int i = 0; i < solutions.Length; i++)
			results[i] = false;
		
		if(placedObject == null) return results;
		
		for(int j = 0; j < solutions.Length; j++)
		{
			results[j] = solutions[j].IsEqualTo(placedObject.GetObjectState());
		}
		return results;
	}
}
