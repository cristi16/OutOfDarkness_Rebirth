using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
public class GhostAnimationController : MonoBehaviour
{
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed

	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer

	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int walkState = Animator.StringToHash("Base Layer.Walk");	
	static int walkBackState = Animator.StringToHash("Base Layer.Walk Back");
	
	internal bool hasControl = true;
	private NavMeshAgent agent;

	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();
		agent = transform.parent.GetComponent<NavMeshAgent>();
	}
	
	void FixedUpdate ()
	{
		if(hasControl == false)
		{
			anim.SetFloat("Speed", agent.velocity.sqrMagnitude / agent.speed * agent.speed);
			return;
		}
		
		float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
		float h = Input.GetAxis("Horizontal");
		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis	
		anim.SetFloat("Direction", h);	
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
				
	}
	
	public void ResetToIdle()
	{
		hasControl = false;
		if( anim != null)
		{
			anim.SetFloat("Speed", 0);
			anim.SetFloat("Direction", 0);	
		}
	}
}
