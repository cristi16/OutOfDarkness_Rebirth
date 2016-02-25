using UnityEngine;
using System.Collections;

public class LosingCondition : MonoBehaviour {
	
	private GameObject player;
	private CheckpointsManager_Script gameManager;
	
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Kid");
		GameObject game_manager = GameObject.Find("Game Manager");
		
		if(player == null || game_manager == null){
			Debug.LogError("Error inizialization of trigger AI script "+this.name);
			return;
		}
		
		gameManager = game_manager.GetComponent<CheckpointsManager_Script>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider other) {
        if (other.CompareTag("Kid") ){
			RaycastHit hit;
			Vector3 targetDir = (player.transform.position + new Vector3(0, player.GetComponent<CharacterController>().height / 2, 0) ) - transform.position;
	        if (Physics.Raycast(transform.position,  targetDir, out hit) && hit.collider==player.GetComponent<Collider>()){
//				chaseOn=true;
//				agent.speed=chase_speed;
				
				// reset position of ghost and girl

				gameManager.ResetLevel();
				
			}
		}
    }
}
