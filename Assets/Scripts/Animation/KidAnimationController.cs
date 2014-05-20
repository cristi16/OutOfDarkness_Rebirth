using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
public class KidAnimationController : MonoBehaviour
{
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed

	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer

	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int walkState = Animator.StringToHash("Base Layer.Walk");	
	static int sneakingState = Animator.StringToHash("Base Layer.Sneaking");// these integers are references to our animator's states
	static int walkBackState = Animator.StringToHash("Base Layer.Walk Back");
	static int strafeRightState = Animator.StringToHash("Base Layer.Strafe_Right");
	static int strafeLeftState = Animator.StringToHash("Base Layer.Strafe_Left");
	
	private TP_Controller controller;
	internal bool hasControl = true;

	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();					  
		
		controller = transform.parent.GetComponent<TP_Controller>();
	}
	
	void FixedUpdate ()
	{
		/*if(hasControl == false) return;
		
		float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
		float h = Input.GetAxis("Horizontal");
		
		if(controller!=null && controller.walkOnlyForward)
		{
			if(v < 0) v = 0;
			h = 0;
		}
		
		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis
		anim.SetFloat("Direction", h);
		//anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
		if(Input.GetButton("Sneak"))
		{
			if(currentBaseState.nameHash == strafeRightState || currentBaseState.nameHash == strafeLeftState )
				anim.speed = animSpeed * controller.player_sneak.sneakingMultiplier;
			
			anim.SetBool("Sneaking", true);
		}
		else
		{
			if(currentBaseState.nameHash != strafeRightState)
				anim.speed = animSpeed;
			
			anim.SetBool("Sneaking", false);	
		}
		
		*/
			
	}
	
	public void ResetToIdle()
	{
		hasControl = false;
		anim.SetFloat("Speed", 0);
		anim.SetFloat("Direction", 0);
	}
}
