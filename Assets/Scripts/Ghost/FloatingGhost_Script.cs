using UnityEngine;
using System.Collections;

public class FloatingGhost_Script : MonoBehaviour {
	
	public GameObject target;
	
	public float y_acceleration;
	public float y_max_speed;
	
	private bool controlling_ghost; //Changed by the switching mechanic every time they switch
	
	private float y_speed;
	private bool y_ascending;
	
	private Vector3 total_floating_movement;
	
	private Vector3 initial_position;
	
	private NavMeshAgent agent;
	
	private float nextfloat;
	
	private GameObject kid;
	private GameObject ghost;
	
	private float time_between=0.01f;
	
	// Use this for initialization
	void Start () {
		y_ascending = true;
		controlling_ghost = false;
		
		total_floating_movement = Vector3.zero;
		initial_position = this.transform.position - target.transform.position;
		
		agent = GetComponent<NavMeshAgent>();
		
		if(agent == null){
			Debug.LogError("Error inizialization of agent, Floating ghost script");
			return;
		}
		
		kid = GameObject.FindGameObjectWithTag("Kid");
		ghost = GameObject.FindGameObjectWithTag("Ghost");
		
	}
	
	// Update is called once per frame
	void Update () {	
		if(!controlling_ghost && Time.time > nextfloat){
			
			//if(LevelState.getInstance().ghostFollowsKidFromStart)
			//	agent.destination = target.transform.position;
			
			if(y_acceleration != 0){			
				if(agent!=null && agent.enabled){
					if(Vector3.Distance(kid.transform.position,ghost.transform.position)<10){					
						agent.speed=3;
					} else {
						agent.speed=4;
					}
				}
				
				if(y_ascending && y_speed < y_max_speed){
					y_speed+=y_acceleration;
				}else if(!y_ascending && y_speed > -y_max_speed){
					y_speed-=y_acceleration;
				}
				
				if(y_ascending && y_speed >= y_max_speed){
					y_ascending = false;
				}else if (!y_ascending && y_speed <= -y_max_speed){
					y_ascending = true;
				}
				
				agent.baseOffset += y_speed;
			}
			
			nextfloat = time_between + Time.time;
		}
	}
	
	public bool getControllingGhost(){
		return controlling_ghost;
	}
	
	public void setControllingGhost(bool temp){
		controlling_ghost = temp;
	}
}
