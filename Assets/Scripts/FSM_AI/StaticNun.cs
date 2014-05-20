using UnityEngine;
using System.Collections;

public class StaticNun : NunStateMachine {
	
	private Vector3 static_spot;
	private Quaternion static_rotation;
	
	// Use this for initialization
	protected override void OnAwake()
	{
		static_spot = transform.position;
		static_rotation = transform.rotation;
		base.OnAwake();
	}
	
	#region Default
	protected override IEnumerator Default_EnterState()
	{
		agent.destination = static_spot;
		lookingAround = false;
		relativeForwardRotation = static_rotation;
		agent.speed = patrolSpeed;		
		yield return null;
	}
	
	protected override void Default_Update ()
	{
		if(agent.velocity == Vector3.zero) //Has reached the static point
		{ 
			if (!lookingAround) //Has reached the static point
			{
				if(Quaternion.Angle(transform.rotation, static_rotation) > 2f)
					transform.rotation = Quaternion.Slerp(transform.rotation, static_rotation, Time.deltaTime * 2);
				else
					lookingAround = true;
			}
			else //Looking left and right while static
			{ 
				LookAround();
			}
		}
		
		base.Default_Update();
	}
	#endregion
	
}
