using UnityEngine;
using System.Collections;

public class SlideCurtains : MonoBehaviour {
	
	public Transform[] attachedColliders;
	public Transform finalPoint;
	public bool slideCurtains = false;
	internal bool opened  = false;
	public float speed = 1f;
	public float finalPointOffset = 0.2f;
	public float triggerDistance = 0.3f;
	
	void Start () {
	}
	
	void Update () {
		if(slideCurtains)
		{
			for(int i = 0; i < attachedColliders.Length - 1; i++)
			{	
				if(attachedColliders[i].tag == "CurtainTriggered")
				{
					attachedColliders[i].position = Vector3.Lerp(attachedColliders[i].position, 
						finalPoint.position + new Vector3(i * finalPointOffset, 0, 0), Time.deltaTime * speed);	
				}
				else if(Vector3.Distance(attachedColliders[i - 1].position, attachedColliders[i].position) < triggerDistance)
				{
					attachedColliders[i].tag = "CurtainTriggered";
				}
			}
			
			if( Vector3.Distance( attachedColliders[0].position, finalPoint.position) < 0.001f )
			{
				slideCurtains = false;
				opened = true;
			}
		}
	}
}
