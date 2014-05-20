using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
public class SittingStandingNunController : MonoBehaviour
{
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed

	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer

	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int walkState = Animator.StringToHash("Base Layer.Walk");
	static int sittingState = Animator.StringToHash("Base Layer.Sitting");
	static int standingState = Animator.StringToHash("Base Layer.Standing");
	
	private NavMeshAgent agent;
	private SleepingNunFSM ai;

	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();
		agent = transform.parent.GetComponent<NavMeshAgent>();
		ai = transform.parent.GetComponent<SleepingNunFSM>();
	}
	
	void FixedUpdate ()
	{
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		
		anim.SetFloat("Speed", agent.velocity.sqrMagnitude / agent.speed * agent.speed);
		anim.SetBool("Sitting", ai.getSitting());
		anim.SetBool("Standing", ai.getStanding());
		
		//if(ai.getStanding()) Debug.Log("standing");
				
		if(currentBaseState.nameHash == sittingState)
			ai.setSitting(false);
		if(currentBaseState.nameHash == standingState)
			ai.setStanding(false);
				
	}
}
