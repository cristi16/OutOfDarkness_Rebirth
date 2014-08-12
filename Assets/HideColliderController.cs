using UnityEngine;
using System.Collections;

public class HideColliderController : MonoBehaviour {
	private BoxCollider bedCollider;
	private SneakWalkRunController sneak;

	private Vector3 sneakingColliderCenter;
	private Vector3 normalColliderCenter;
	private HidingController hc;
	private bool firstTime=true;
	private static float scaleMultiplier=3f;
	private float localScaleY;
	private Vector3 auxLocalScale;
	public float hidingPositionOffset=0f;
	public float hidingPerspectiveOffset=0f;

	// Use this for initialization
	void Start () {
		bedCollider = transform.parent.GetComponent<BoxCollider> ();
		sneak = GameObject.FindGameObjectWithTag ("Kid").GetComponent<SneakWalkRunController> ();
		hc = GameObject.FindGameObjectWithTag ("Kid").GetComponent<HidingController> ();

		normalColliderCenter = new Vector3 (bedCollider.center.x,bedCollider.center.y,bedCollider.center.z);
		sneakingColliderCenter = new Vector3 (bedCollider.center.x,bedCollider.size.y,bedCollider.center.z);
		localScaleY = transform.parent.localScale.y;
	}
	
	// Update is called once per frame
	void Update () {
		/*if (hc.hidingSpot != transform)
						return;

		if (hc.hiding) {
			if(transform.parent.localScale.y<localScaleY*scaleMultiplier){
				auxLocalScale.y+=Time.deltaTime;
				transform.parent.localScale=auxLocalScale;
			}
		} else {
			if(transform.parent.localScale.y>localScaleY){
				auxLocalScale.y-=Time.deltaTime;
				transform.parent.localScale=auxLocalScale;
			}
		}*/

	}

	void OnTriggerStay(Collider other){
		if (other.tag == "Kid" && sneak.getSneak() && sneak.Moving()) {
			//bedCollider.center=sneakingColliderCenter;
			if(firstTime){
				bedCollider.enabled=false;
				sneak.canGetUp=false;
				hc.hiding=true;
				auxLocalScale=transform.parent.localScale;
				hc.hidingSpot=transform;
				firstTime=false;

				/*foreach(CameraDisableClipping c in Camera.main.transform.parent.GetComponentsInChildren<CameraDisableClipping>()){
					c.hidingOffset=hidingOffset;
					c.EnableHiding();
				}*/

				foreach(Camera c in Camera.main.transform.parent.GetComponentsInChildren<Camera>()){
					if(c.GetComponent<Headbobber>()!=null){
						c.GetComponent<Headbobber>().hidingMidpointLimit+=hidingPositionOffset;
						c.GetComponent<Headbobber>().StartPerspectiveChange(hidingPerspectiveOffset);
					}
				}
			}
				
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Kid" && hc.hiding) {
			//bedCollider.center=normalColliderCenter;
			bedCollider.enabled=true;
			sneak.canGetUp=true;
			hc.hiding=false;
			firstTime=true;

			/*foreach(CameraDisableClipping c in Camera.main.transform.parent.GetComponentsInChildren<CameraDisableClipping>()){
				c.DisableHiding();
			}*/

			foreach(Camera c in Camera.main.transform.parent.GetComponentsInChildren<Camera>()){
				if(c.GetComponent<Headbobber>()!=null){
					c.GetComponent<Headbobber>().hidingMidpointLimit-=hidingPositionOffset;
					c.GetComponent<Headbobber>().RevertPerspectiveChange();
				}
			}
		}
	}

}
