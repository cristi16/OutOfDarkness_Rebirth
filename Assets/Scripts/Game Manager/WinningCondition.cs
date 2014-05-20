using UnityEngine;
using System.Collections;

public class WinningCondition : MonoBehaviour {
	
	private CheckpointsManager_Script end_level;
	
	// Use this for initialization
	void Start () {
		GameObject game_manager = GameObject.Find("Game Manager");
		
		if(game_manager == null){
			Debug.LogError("Error inizialization of trigger AI script "+this.name);
			return;
		}
		
		end_level = game_manager.GetComponent<CheckpointsManager_Script>();
	}
	
		void OnTriggerStay(Collider other) {
        if (other.CompareTag("Kid"))
		{	
			// reset position of ghost and girl
			if(Input.GetButtonDown("Interaction"))
				LevelState.getInstance().ClearLevelState();
				end_level.ResetLevel();		
			
		}
    }
}
