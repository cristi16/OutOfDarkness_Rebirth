using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {
	
	//public static TP_Motor instance;
	
	public float forwardSpeed = 10f;
	public float backwardSpeed = 2f;
	public float strafingSpeed = 5f;

	public static bool oculusRift = true;
	
	
	private SneakWalkRunController sneak_script;
	
	internal TP_Controller controller;
	
	public Vector3 moveVector { get; set; }
	
	public void Start(){
		
		sneak_script = gameObject.GetComponent<SneakWalkRunController>();	
	}
	
	void Awake() 
	{
		//instance=this;
		controller = gameObject.GetComponent<TP_Controller>();
	}
	
	public void UpdateMotor() 
	{
		SnapAlignCharacterWithCamera();
		ProcessMotion();
		RayCastForColliders();
	}

	void RayCastForColliders(){
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Camera.main.transform.forward, out hit)) {
			InteractiveCollider col = hit.collider.GetComponent<InteractiveCollider>();
			ActionOnSight act = hit.collider.GetComponent<ActionOnSight>();
			if(col!=null) col.SendMessage("OnMouseOver");
			if(act!=null) act.SendMessage("OnMouseOver");
		}

	}

	void ProcessMotion()
	{
		// Transform our moveVector into World Space relative to our character rotation
		moveVector = Camera.main.transform.TransformDirection(moveVector);
		
		// Normalize moveVector if magnitude is > 0
		if(moveVector.magnitude > 1)
			moveVector = Vector3.Normalize(moveVector);
		
		// Multiply moveVector by moveSpeed
		moveVector *= MoveSpeed();
		
		
		if(gameObject.tag == "Kid"){	
		// Move the Character in World Space
			controller.characterController.SimpleMove(moveVector);
		}else{
			// Multiply moveVector by DeltaTime
			moveVector *= Time.deltaTime;
			controller.characterController.Move(moveVector);
			
		}
	}
	
	void SnapAlignCharacterWithCamera()
	{
		if(moveVector.x != 0 || moveVector.z != 0)
		{
			/*transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 
				Camera.mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);*/
		}
	}
	
	private float MoveSpeed()
	{
		float moveSpeed = 0f;
		
		switch (controller.MoveDirection)
		{
			case TP_Controller.Direction.Stationary:
				moveSpeed = 0;
				break;
			case TP_Controller.Direction.Forward:
				moveSpeed = forwardSpeed;
				break;
			case TP_Controller.Direction.Backward:
				moveSpeed = backwardSpeed;
				break;
			case TP_Controller.Direction.Left:
				moveSpeed = strafingSpeed;
				break;
			case TP_Controller.Direction.Right:
				moveSpeed = strafingSpeed;
				break;
			case TP_Controller.Direction.LeftForward:
				moveSpeed = forwardSpeed;
				break;
			case TP_Controller.Direction.RightForward:
				moveSpeed = forwardSpeed;
				break;
			case TP_Controller.Direction.LeftBackward:
				moveSpeed = backwardSpeed;
				break;
			case TP_Controller.Direction.RightBackward:
				moveSpeed = backwardSpeed;
				break;
		}
		// if we are controlling the kid and she runs or sneaks
		if(sneak_script != null && sneak_script.getRun())
		{		
			moveSpeed*=sneak_script.runningMultiplier;
		} else if(sneak_script != null && sneak_script.getSneak())
		{		
			moveSpeed*=sneak_script.sneakingMultiplier;
		}
		return moveSpeed;
	}
}
