using UnityEngine;
using System.Collections;

public class ActivateNunInvestigation : MonoBehaviour {

	public NunStateMachine nun;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddNun(){
		NunAlertManager.getInstance().AddNun(nun,false);
	}

	void RemoveNun(){
		NunAlertManager.getInstance().RemoveNun(nun,false);
	}

	void OnTriggerEnter(){
		Invoke ("AddNun",10f);
		Invoke ("RemoveNun",20f);
	}
}
