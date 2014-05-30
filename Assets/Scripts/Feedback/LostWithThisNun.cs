using UnityEngine;
using System.Collections;

public class LostWithThisNun : MonoBehaviour {
	
	private GameObject player;
	private LevelState levelState;	
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Kid");		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SaveLoss(){
		LevelState.getInstance().SaveLoss(transform.parent.GetComponentInChildren<NunStateMachine>());					
	}

	void OnTriggerEnter(Collider other) {        
    }
}
