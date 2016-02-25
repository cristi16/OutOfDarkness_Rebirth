using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HidingController : MonoBehaviour {
	
	internal bool hiding=false;
	internal bool comingOut=false;
	
	private float time=0f;
	private float timeToComeOut=2f;

	private bool firstTime=true;
	
	private Vector3 sunrise;
	private Vector3 sunset;
	private float slerpStartTime;
	private Transform hidingPoint;
	internal Transform hidingSpot;
	private Transform outPoint;
	private float hidingSpotsRange;
	public static bool rotateHeadAfterHiding = false;
	
	public float normalFieldOfView=90f;
	public float hidingFieldOfView=10.0f;
	public bool reduceFieldOfView=false;
	
	internal bool hidingMode=true;	
	private bool justChanged=true;
	
	private Quaternion baseRotation;
	private TP_Controller controller;
	private Transform nun;
	
	public bool isHiddenOrHidingOrComingOut(){
		return hiding || comingOut;
	}
	
	// Use this for initialization
	void Start () {
		if(gameObject.transform.FindChild("HidingSpotsRange")!=null)
			hidingSpotsRange = gameObject.transform.FindChild("HidingSpotsRange").GetComponent<SphereCollider>().radius;
		else
			hidingSpotsRange = 25f;
		controller = GameObject.FindGameObjectWithTag ("Kid").GetComponent<TP_Controller> ();
	}		
	
	// Update is called once per frame
	void Update () {
		if (comingOut) {
			if(firstTime){
				time=Time.time;

				sunrise = controller.transform.position;
				sunset = nun.position+new Vector3(0f,2f,0f);
				firstTime=false;
			}

			// The center of the arc
			Vector3 center = (sunrise + sunset) * 0.5f;
			// move the center a bit to the side to make the arc horizontal
			center -= new Vector3(0, 1, 1);
			// Interpolate over the arc relative to center
			Vector3 riseRelCenter = sunrise - center;
			Vector3 setRelCenter = sunset - center;
			// The fraction of the animation that has happened so far is
			// equal to the elapsed time divided by the desired time for
			// the total journey.

			float fracComplete = (Time.time - time) / timeToComeOut;
			controller.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
			controller.transform.position += center;
		}
	}

	public void ComeOut(Transform nun){
		if (!comingOut) {
			GameObject.FindGameObjectWithTag ("Kid").GetComponent<TP_Controller> ().removeControl ();
			comingOut = true;
			this.nun = nun;
		}
	}

	public List<Vector3> GetHidingSpotsInRange()
	{
		List<Vector3> hidingSpots = new List<Vector3>();
		RaycastHit hitInfo;
		//disable current hiding spot collider
		if (hidingSpot != null) {
			hidingSpot.parent.GetComponent<BoxCollider>().enabled = false;
			
			int layerMask = 1 << LayerMask.NameToLayer("HidingSpots");
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, hidingSpotsRange, layerMask);
			
			layerMask += 1 << LayerMask.NameToLayer("Wall");
			foreach(Collider hit in hitColliders)
			{
				if(hit != hidingSpot.GetComponent<Collider>())
				{
					if(Physics.Raycast(transform.position, hit.transform.position - transform.position,
					                   out hitInfo, hidingSpotsRange, layerMask))
					{
						if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("HidingSpots") 
						   && hitInfo.collider == hit)
							hidingSpots.Add(hitInfo.transform.position);
					}
				}	
			}
			//hidingSpots.Add(GetCurrentHidingSpot());
			hidingSpot.GetComponent<Collider>().enabled = true;
		}
		return hidingSpots;
	}
	
	public Vector3 GetCurrentHidingSpot()
	{
		return hidingSpot.position;
	}

}
