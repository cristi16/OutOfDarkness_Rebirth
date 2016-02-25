using UnityEngine;
using System.Collections;

public class RemoveVent : MonoBehaviour {
	internal bool activated = false;
	public Material openVentMaterial;
	
	private InteractiveTrigger trigger;
	private InteractiveCollider collider;
	
	// Use this for initialization
	void Start () {
		if(activated){
			if(GetComponent<Renderer>()!=null) GetComponent<Renderer>().material = openVentMaterial;
		}
		trigger = GetComponent<InteractiveTrigger>();
		collider = GetComponent<InteractiveCollider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider col)
	{		
		if(col.CompareTag("Kid") && ((trigger!=null && trigger.getGui()) || (collider!=null && collider.activateHelpCondition())))
		{
		
			if(!activated)
			{
				activated = true;				
				GetComponent<Renderer>().material = openVentMaterial;		
			}
		}
	}
}
