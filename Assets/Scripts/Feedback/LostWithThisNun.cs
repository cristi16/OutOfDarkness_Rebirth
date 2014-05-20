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
	
	void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Kid") ){
			RaycastHit hit;
			Vector3 targetDir = player.transform.position - transform.position;
	        if (Physics.Raycast(transform.position,  targetDir, out hit) && hit.collider==player.collider){
				LevelState.getInstance().SaveLoss(transform.parent.GetComponentInChildren<NunStateMachine>());					
			}
		}
    }
}
