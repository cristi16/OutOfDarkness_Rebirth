using UnityEngine;
using System.Collections;

public class PlaceableObject : MonoBehaviour {
	
	internal bool selected;
	public int id;
	
	[Multiline]
	public string hintText;
	
	private PuzzleController puzzleController;
	
	// Use this for initialization
	void Start () {
	 	puzzleController = transform.parent.parent.GetComponent<PuzzleController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(selected)
		{
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
				puzzleController.cameraStopDistance  - puzzleController.pickUpOffset);

    		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
			transform.position = curPosition;
		}
	}
	
	public virtual ObjectState GetObjectState()
	{
		return new ObjectState(id);	
	}
	

	
	public virtual void PerformRightClick()
	{		
	}
	
	
}
