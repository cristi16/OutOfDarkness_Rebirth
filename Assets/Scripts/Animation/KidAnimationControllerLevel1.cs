using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
public class KidAnimationControllerLevel1 : KidAnimationController
{

	void Start ()
	{
		base.Start ();
	}
	
	void FixedUpdate ()
	{
		if (controller==null || controller.hasControl == false)
						return;
		if (moving) {
			if(firstTime){
				transform.root.GetComponentInChildren<LookingAt>().enabled=false;
				anim.SetFloat("Speed", 0.5f);
				GetComponent<NavMeshAgent> ().SetDestination (GameObject.FindGameObjectWithTag("KidDestination").transform.position);
				firstTime=false;
				transform.parent.audio.Play();
			}		
		}


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
		
		if(Input.GetButton())
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
		anim.SetFloat("Speed", 0);
		anim.SetFloat("Direction", 0);
	}

	public override void Walk(){
		if (use) {
						moving = true;
						use=false;
				}
	}

	public override void Stop(){
		GetComponent<NavMeshAgent>().Stop();
		ResetToIdle ();
		transform.root.GetComponentInChildren<LookingAt>().enabled=true;
	}
}
