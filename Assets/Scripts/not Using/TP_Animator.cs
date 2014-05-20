using UnityEngine;
using System.Collections;

public class TP_Animator : MonoBehaviour 
{
	public enum Direction
	{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}
	
	private TP_Motor motor;
	
	public Direction MoveDirection { get; set; }
	
	void Awake()
	{
		motor = gameObject.GetComponent<TP_Motor>();
	}
	
	void Update()
	{
	
	}
	
	public void DetermineCurrentMoveDirection()
	{
		bool forward = false;
		bool backward = false;
		bool left = false;
		bool right = false;
		
		if (motor.moveVector.z > 0)
			forward = true;
		if (motor.moveVector.z < 0)
			backward = true;
		if (motor.moveVector.x > 0)
			right = true;
		if (motor.moveVector.x < 0)
			left = true;
		
		if (forward)
		{
			if (left)
				MoveDirection = Direction.LeftForward;
			else if (right)
				MoveDirection = Direction.RightForward;
			else
				MoveDirection = Direction.Forward;
		}
		else if (backward)
		{
			if (left)
				MoveDirection = Direction.LeftBackward;
			else if (right)
				MoveDirection = Direction.RightBackward;
			else
				MoveDirection = Direction.Backward;
		}
		else if (left)
			MoveDirection = Direction.Left;
		else if (right)
			MoveDirection = Direction.Right;
		else
			MoveDirection = Direction.Stationary;	
	}
}
