using UnityEngine;
using System.Collections;

public class ChasingOnDistance_Script : MonoBehaviour {
	
	private SneakWalkRunController player_sneak;
	private HidingController hidingController;
	private NunStateMachine nun_ai;
	private int layerMask = 0;
	private RaycastHit hitInfo;
	
	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindGameObjectWithTag("Kid");
		hidingController = player.GetComponent<HidingController>();
		GameObject nun = transform.parent.gameObject;
		
		if(player == null || nun == null){
			Debug.LogError("Error in the initialization of ChekpointsOnDistance_script");
			return;
		}
		
		// Take into account only the following layers
		layerMask =  1 << LayerMask.NameToLayer("Wall");
		layerMask += 1 << LayerMask.NameToLayer("Doors");
		layerMask += 1 << LayerMask.NameToLayer("Player");
		layerMask += 1 << LayerMask.NameToLayer("GhostCollider");
		
		nun_ai = nun.GetComponent<NunStateMachine>();
		player_sneak = player.GetComponent<SneakWalkRunController>();
	}
	
	void OnTriggerStay(Collider collider){
		
		if(hidingController.hiding) return;
		
		if(transform.parent.CompareTag("Nun"))
		{
			if(collider.CompareTag("Kid") && !player_sneak.getSneak())
			{ //The collider is the kid and it's not sneaking
				
				if(Physics.Raycast(transform.position, collider.transform.position - transform.position , 
					out hitInfo, Mathf.Infinity, layerMask))
				{		
					if(LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "Player")
					{
						nun_ai.ActivateDistractionInvestigation(collider.transform);
						//Debug.Log("activatedDistraction");
					}
				}
			}	
		}
	}
}
