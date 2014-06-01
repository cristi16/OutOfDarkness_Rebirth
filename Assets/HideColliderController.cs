using UnityEngine;
using System.Collections;

public class HideColliderController : MonoBehaviour {
	private BoxCollider bedCollider;
	private SneakWalkRunController sneak;

	private Vector3 sneakingColliderCenter;
	private Vector3 normalColliderCenter;
	private HidingController hc;
	private bool firstTime=true;

	// Use this for initialization
	void Start () {
		bedCollider = transform.parent.GetComponent<BoxCollider> ();
		sneak = GameObject.FindGameObjectWithTag ("Kid").GetComponent<SneakWalkRunController> ();
		hc = GameObject.FindGameObjectWithTag ("Kid").GetComponent<HidingController> ();

		normalColliderCenter = new Vector3 (bedCollider.center.x,bedCollider.center.y,bedCollider.center.z);
		sneakingColliderCenter = new Vector3 (bedCollider.center.x,bedCollider.size.y,bedCollider.center.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other){
		if (other.tag == "Kid" && sneak.getSneak() && sneak.Moving()) {
			//bedCollider.center=sneakingColliderCenter;
			if(firstTime){
				bedCollider.enabled=false;
				sneak.canGetUp=false;
				hc.hiding=true;
				hc.hidingSpot=transform;
				firstTime=false;
			}
				
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Kid") {
			//bedCollider.center=normalColliderCenter;
			bedCollider.enabled=true;
			sneak.canGetUp=true;
			hc.hiding=false;
			firstTime=true;
		}
	}

}
