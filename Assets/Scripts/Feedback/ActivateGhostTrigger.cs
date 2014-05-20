using UnityEngine;
using System.Collections;

public class ActivateGhostTrigger : MonoBehaviour {
	
	SwitchMechanic_Script switchMechanic;
	
	// Use this for initialization
	void Start () {
		switchMechanic = GameObject.FindGameObjectWithTag("GameController").GetComponent<SwitchMechanic_Script>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private void OnTriggerEnter(Collider hit){
	
		if(hit.collider.tag == "Kid")
		{
			//LevelState.getInstance().ghostFollowsKidFromStart = true;
			//LevelState.getInstance().swapActivatedFromStart = true;
			switchMechanic.swapActivated = true;
			switchMechanic.kid.GetComponent<LookForGhost>().enabled = true;
			Destroy(this.gameObject);
		}
	}
}
