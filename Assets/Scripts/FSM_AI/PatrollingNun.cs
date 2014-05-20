using UnityEngine;
using System.Collections;

public class PatrollingNun : NunStateMachine {
	
	public GameObject[] nodes;
	//Human behaviour (sort of)
	public float multiplier_while_waiting = 1.2f;
	public float multiplier_while_walking = 8;
	public float random_wait_min_time = 8;
	private float next_random_wait;
	private float timer_between_random_wait;
	
	protected override IEnumerator Default_EnterState()
	{
		int closestNode = 0;
		float minDist = 999999f;
		for (int i = 0; i < nodes.Length; i++){
			float temp = Vector3.SqrMagnitude(nodes[i].transform.position - transform.position);
			if (temp < minDist) {
				minDist = temp;
				closestNode = i;
			}
		}
		currentNode = closestNode;
		agent.destination = nodes[currentNode].transform.position;
		isWaiting = false;
		agent.speed = patrolSpeed;		
		yield return null;
	}
	
	// Update is called once per frame
	protected override void Default_Update()
	{
		if(!isWaiting)
		{
			float distanceX = Mathf.Abs(transform.position.x - agent.destination.x);
			float distanceZ = Mathf.Abs(transform.position.z - agent.destination.z);

			if (distanceX <= agent.stoppingDistance && distanceZ <= agent.stoppingDistance)
			{				
				if (++currentNode == nodes.Length)
					currentNode=0;
			}
			agent.destination=nodes[currentNode].transform.position;
			
			PlayWalkingAudioClip(); //AUDIO: Walking
		}
		
		if(!isWaiting && Time.time > next_random_wait)
		{
			agent.Stop();
			isWaiting=true;
			timer_between_random_wait = Time.time + multiplier_while_waiting * Random.value + 1;
			
			next_random_wait = Time.time + multiplier_while_walking * Random.value 
				+ random_wait_min_time + timer_between_random_wait;
		}
		
		if(isWaiting && Time.time > timer_between_random_wait)
		{
			agent.Resume();
			isWaiting=false;
		}
		
		base.Default_Update();
	}
}
