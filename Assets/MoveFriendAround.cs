using UnityEngine;
using System.Collections;

public class MoveFriendAround : MonoBehaviour {

	public Transform node;
	public bool stop;
	public bool wait=false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator OnTriggerEnter(Collider col){
		if (col.tag == "Friend") {
			if(stop){
				col.GetComponent<KidAnimationController>().Stop();
				col.transform.root.GetComponentInChildren<ActionOOD>().execute();
			} else if(wait){
				col.GetComponent<KidAnimationController>().anim.SetFloat("Speed", 0f);
				yield return new WaitForSeconds(1f);
				col.GetComponent<KidAnimationController>().anim.SetFloat("Speed", 0.5f);
				col.GetComponent<NavMeshAgent>().SetDestination(node.position);
				enabled=false;
			} else {
				col.GetComponent<NavMeshAgent>().SetDestination(node.position);
				enabled=false;
			}
				}
	}
}
