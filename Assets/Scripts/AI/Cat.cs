using UnityEngine;
using System.Collections;

public class Cat : AI {
	
	public float cat_distance_before_stopping = 10;
	public float waiting_time_after_cat = 10;
	public float nun_call_range=100f;
	GameObject[] nuns;
	
	void Start(){
		base.Start();
		
		GameObject temp = GameObject.Find("Ghost");
		if(temp == null){
			Debug.LogError("Error in cat initialization");
			return;
		}
		setPlayer(temp);
		
		nuns = GameObject.FindGameObjectsWithTag("Nun");
	}
	
	void Update(){
		if(base.getChase() || base.getInvest())
			AttractNuns();	
		
		base.Update();
		
	}
	
	void AttractNuns() {
		AI nunAI;
		float min_distance = nun_call_range + 1;
		int index = -1;
		
		if(nuns.Length > 0){
			for (int i=0; i < nuns.Length; i++){
				float temp_distance = Vector3.Distance(nuns[i].transform.position, transform.position);
				if ( temp_distance < min_distance){
					min_distance = temp_distance;
					index = i;
				}
			}
			
			if(index != -1){
				nunAI = nuns[index].GetComponent<AI>();
				nunAI.activateNormalInvestigate(transform.gameObject);
				nunAI.setInvestigatingDistance(cat_distance_before_stopping);
				nunAI.setTimeAfterDistraction(waiting_time_after_cat);
			}
		}	
	}
	
}
