using UnityEngine;
using System.Collections;

public enum PuzzleType
{
	Sorting = 1,
	Mixing = 2
}
[RequireComponent(typeof(SphereCollider))]
public class PuzzleController : MonoBehaviour {
	
	public PuzzleType puzzleType = PuzzleType.Sorting;
	public bool requiresAllItems = false;
	public int numberOfSolutions = 1;
	public bool exclusiveSolutions = true;
	public bool showHints = false;
	
	public float cameraStopDistance = 1f;
	internal float cameraJourneyTime = 1f;
	public float pickUpOffset = 0.1f;
	internal bool isActivated = false;
	internal PlaceableObject selectedObject;
	internal PuzzleCamera puzzleCamera;
	internal TextMesh hintsText;	
	private Place[] places;
	private int numberOfPlaces;
	private Transform target;
	private bool kidInRange = false;
	internal float timeLastDisable=0f;
	internal bool canExit=true;	
	
	public bool deactivateAtExit=false;
	public Action actionWhenSolved;
	
	// Use this for initialization
	void Start () {		
		places = GetComponentsInChildren<Place>();
		numberOfPlaces = GetNumberOfPlaces(places);
		puzzleCamera = Camera.mainCamera.transform.parent.root.GetComponentInChildren<PuzzleCamera>();
		target = transform.FindChild("LookAtPoint");
		GetComponent<SphereCollider>().isTrigger = true;
		
		foreach(Place place in places)
			place.collider.enabled = false;
		
		if(showHints)
			hintsText = transform.FindChild("Hints").GetComponent<TextMesh>();
	}
	
	public void CheckForSolution()
	{
		int numberOfPlacedItems = GetNumberOfPlacedItem(places);
		
		// check if the puzzle requires all items to be placed in order for it to be solved
		if(requiresAllItems && numberOfPlacedItems != numberOfPlaces) return;
		
		bool hasSolution;
		if(exclusiveSolutions)
			hasSolution = CheckExclusiveSolution();
		else
			hasSolution = CheckNonExclusiveSolution();
		if(hasSolution)
		{
				Debug.Log("found solution");
				// DO SOMETHING
							
				if(actionWhenSolved!=null) actionWhenSolved.execute();								
			
				return;
		}
		
		// Solution not found
		if(puzzleType == PuzzleType.Sorting)
		{
			//Do nothing	
			Debug.Log("Sorting result: no solution");
		}
		else if(puzzleType == PuzzleType.Mixing)
		{
			// check if all items have been placed
			if(numberOfPlacedItems == numberOfPlaces)
			{
				Debug.Log("Mixing result: no solution");
			}
		}
	}
	
	private bool CheckExclusiveSolution()
	{
		bool[] possibleSolutions = new bool[numberOfSolutions];
		for(int i = 0; i < possibleSolutions.Length; i++)
			possibleSolutions[i] = true;
		
		for(int i = 0; i < places.Length; i++)
		{
			if(places[i].isDefaultPlace || places[i].isEmptyPlace) continue;
			
			bool[] evalutions = places[i].GetSolutionEvaluation();
			
			for(int j = 0; j < possibleSolutions.Length; j++)
				possibleSolutions[j] = possibleSolutions[j] && evalutions[j];
		}
		
		bool hasSolution = false;
		
		foreach( bool temp in possibleSolutions)
		{	
			if(temp == true)
				return true;
		}
		
		return false;
	}
	
	private bool CheckNonExclusiveSolution()
	{
		bool[] possibleSolutions = new bool[numberOfSolutions];
		for(int i = 0; i < possibleSolutions.Length; i++)
			possibleSolutions[i] = true;
		
		bool puzzleHasSolution = true;
		
		for(int i = 0; i < places.Length; i++)
		{
			if(places[i].isDefaultPlace || places[i].isEmptyPlace) continue;
			
			bool[] evalutions = places[i].GetSolutionEvaluation();
			bool placeHasSolution = false;
			
			for(int j = 0; j < evalutions.Length; j++)
				placeHasSolution = placeHasSolution || evalutions[j];
			
			puzzleHasSolution = puzzleHasSolution && placeHasSolution;
			
			if(puzzleHasSolution == false)
		 		return false;
		}
		return true;
	}
	
	internal void CheckPuzzleActivation(){
		if(!isActivated) ActivatePuzzle();
	}
	
	private void Update()
	{
		
		if(kidInRange)
		{
			if(Input.GetButtonUp("Interaction") && LevelState.getInstance().inPlay)
			{
				/*if(isActivated)
					DeactivatePuzzle();
				else 
					ActivatePuzzle();*/
				CheckPuzzleActivation();
			}
			
			//if(Input.GetKeyDown(KeyCode.Escape) && isActivated)
				//Invoke("DeactivatePuzzle",0.1f);
		}
		
		if(isActivated)
		{
			if(selectedObject != null && Input.GetMouseButtonDown(1))
				selectedObject.PerformRightClick();
		}
	}
	
	private void OnTriggerStay(Collider collider)
	{
		if(collider.tag == "Kid")
		{
			kidInRange = true;
		}
	}
	
	private void OnTriggerExit(Collider collider)
	{
		if(collider.tag == "Kid")
		{
			kidInRange = false;
		}
	}
	
	public void ActivatePuzzle()
	{
		isActivated = true;

		if(TP_Motor.oculusRift){

		} else {
			Screen.lockCursor=false;
			Screen.showCursor=true;		
		}
		LevelState.getInstance().puzzleMode = true;
		puzzleCamera.ActivateLookAt(target, cameraStopDistance, cameraJourneyTime);
		foreach(Place place in places)
			place.collider.enabled = true;
		
		if(GetComponentInChildren<InteractiveTrigger>()!=null){
			GetComponentInChildren<InteractiveTrigger>().enabled=false;
			GetComponentInChildren<InteractiveCollider>().enabled=false;
			GetComponentInChildren<InteractiveCollider>().collider.enabled=false;
		}
	}		
	
	public void DeactivatePuzzle()
	{
		if(isActivated){
			ResetPuzzle();
			StartCoroutine(SetActivated(false));
			Screen.lockCursor=true;
			Screen.showCursor=false;
			LevelState.getInstance().puzzleMode = false;
			puzzleCamera.DeactivateLookAt();
			foreach(Place place in places)
				place.collider.enabled = false;
			if(GetComponentInChildren<InteractiveTrigger>()!=null)
				GetComponentInChildren<InteractiveTrigger>().collider.enabled=false;							
		}		
	}
	
	private IEnumerator SetActivated(bool isActivated)
	{
		yield return new WaitForSeconds(0.1f);
		this.isActivated = isActivated;
		LevelState.getInstance().puzzleMode = isActivated;
		
		if(!isActivated){
			EnablePuzzle();
			timeLastDisable = Time.time;
			if(deactivateAtExit) gameObject.SetActive(false);
		}
		yield return null;
	}
	
	public void EnablePuzzle(){
		if(GetComponentInChildren<InteractiveTrigger>()!=null){
			GetComponentInChildren<InteractiveTrigger>().collider.enabled=true;
			GetComponentInChildren<InteractiveTrigger>().enabled=true;
			GetComponentInChildren<InteractiveCollider>().collider.enabled=true;
			GetComponentInChildren<InteractiveCollider>().enabled=true;	
		}
	}
	
	private int GetNumberOfPlacedItem(Place[] places)
	{
		int count = 0;
		
		foreach(Place place in places)
		{
			if(place.isDefaultPlace == false && place.placedObject != null)
				count++;
		}
		return count;
	}
	
	private int GetNumberOfPlaces(Place[] places)
	{
		int count = 0;
		
		foreach(Place place in places)
		{
			if(place.isDefaultPlace == false && place.isEmptyPlace == false)
				count++;
		}
		return count;
	}
	
	internal void ResetPuzzle()
	{
		if(this.selectedObject!=null) this.selectedObject.selected = false;
		this.selectedObject = null;
		foreach(Place place in places)
		{
			if(place.isDefaultPlace == true)
			{
				place.defaultObject.transform.position = place.transform.position;
//				if(puzzleType == PuzzleType.Mixing)
//					place.defaultObject.renderer.enabled = true;
				place.placedObject = place.defaultObject;
				this.selectedObject = null;
			}
			else
			{
				if(puzzleType == PuzzleType.Mixing)
					place.collider.enabled = true;
				place.placedObject = null;	
			}
		}
	}
}
