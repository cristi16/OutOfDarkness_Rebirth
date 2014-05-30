using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HidingController : MonoBehaviour {
	
	internal bool hiding=false;
	internal bool comingOut=false;
	internal bool isHidden = false;
	public float timeToHide=1.5f;
	public float timeToRotate=1.0f;
	
	private float time=0f;
	private bool firstTime=true;
	
	private Vector3 sunrise;
	private Vector3 sunset;
	private float slerpStartTime;
	private Transform hidingPoint;
	internal HidingSpot hidingSpot;
	private Transform outPoint;
	private float hidingSpotsRange;
	public static bool rotateHeadAfterHiding = false;
	
	public float normalFieldOfView=90f;
	public float hidingFieldOfView=10.0f;
	public bool reduceFieldOfView=false;
	
	internal bool hidingMode=true;	
	private bool justChanged=true;
	
	private Quaternion baseRotation;
	
	public bool isHiddenOrHidingOrComingOut(){
		return isHidden || hiding || comingOut;
	}
	
	// Use this for initialization
	void Start () {
		if(gameObject.transform.FindChild("HidingSpotsRange")!=null)
			hidingSpotsRange = gameObject.transform.FindChild("HidingSpotsRange").GetComponent<SphereCollider>().radius;
		else
			hidingSpotsRange = 25f;
	}		
	
	void MoveCamera(Transform sunriseT, Transform sunsetT, bool hiding, float timeToHide){
		if(firstTime){
			baseRotation = new Quaternion(gameObject.transform.rotation.x,gameObject.transform.rotation.y,gameObject.transform.rotation.z,gameObject.transform.rotation.w);
			sunrise = sunriseT.position;
			sunset = sunsetT.position;
			slerpStartTime = Time.time;
			firstTime = false;			
		} else{
			MoveCamera(hiding,timeToHide);
		}		
	}
	
	void MoveCamera(bool hiding, float timeToHide){
		// The center of the arc
		Vector3 center = (sunrise + sunset) * 0.5f;
		// move the center a bit to the side to make the arc horizontal
        center -= new Vector3(0, 0, 1);
		// Interpolate over the arc relative to center
        Vector3 riseRelCenter = sunrise - center;
        Vector3 setRelCenter = sunset - center;
		// The fraction of the animation that has happened so far is
    	// equal to the elapsed time divided by the desired time for
    	// the total journey.
 		float fracComplete = (Time.time - slerpStartTime) / timeToHide;
        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
		if(reduceFieldOfView){				
			if(hiding){		
				if(TP_Motor.oculusRift){
					foreach(Camera c in Camera.main.transform.root.GetComponentsInChildren<Camera>()){
						c.fieldOfView=normalFieldOfView + (hidingFieldOfView - normalFieldOfView)*fracComplete;				
					}
				} else {
					Camera.mainCamera.fieldOfView=normalFieldOfView + (hidingFieldOfView - normalFieldOfView)*fracComplete;				
				}
			} else {
				if(TP_Motor.oculusRift){
					foreach(Camera c in Camera.main.transform.root.GetComponentsInChildren<Camera>()){
						c.fieldOfView=hidingFieldOfView + (-hidingFieldOfView + normalFieldOfView)*fracComplete;
					}
				} else {
					Camera.mainCamera.fieldOfView=hidingFieldOfView + (-hidingFieldOfView + normalFieldOfView)*fracComplete;
				}
			}
		}
	}
	
	void MoveCamera(Vector3 sunriseT, Vector3 sunsetT, bool hiding, float timeToHide){
		if(firstTime){
			baseRotation = new Quaternion(gameObject.transform.rotation.x,gameObject.transform.rotation.y,gameObject.transform.rotation.z,gameObject.transform.rotation.w);
			sunrise = sunriseT;
			sunset = sunsetT;
			slerpStartTime = Time.time;
			firstTime = false;			
		} else{
			MoveCamera(hiding, timeToHide);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(hiding && !comingOut){			
			
			if(time<timeToHide/3f){
				MoveCamera(gameObject.transform.position,new Vector3(gameObject.transform.position.x,hidingPoint.position.y,gameObject.transform.position.z),true,timeToHide/3f);
				time+=Time.deltaTime;
				justChanged=true;
			}else if(time<timeToHide){
				if(justChanged){
					firstTime=true;
					justChanged=false;
				}				
				MoveCamera(gameObject.transform,hidingPoint,true,timeToHide*2f/3f);
				time+=Time.deltaTime;
			} else {
				firstTime=true;
				if(time<timeToHide+timeToRotate){
					time+=Time.deltaTime;
					//Quaternion lookAtRotation = Quaternion.LookRotation(hidingSpot.transform.position - hidingPoint.transform.position);
					//gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, lookAtRotation, (time - timeToHide)/(timeToRotate));					

					if(rotateHeadAfterHiding){
						float yRotation=0; 
						Quaternion orient=Quaternion.identity;
						GetComponentInChildren<OVRCameraController>().GetYRotation(ref yRotation);
						//GetComponentInChildren<OVRCameraController>().GetOrientationOffset(ref orient);
						
						Quaternion lookAtRotation = Quaternion.LookRotation(hidingSpot.transform.position - hidingPoint.transform.position);
						
						GetComponentInChildren<OVRCameraController>().SetYRotation(yRotation-Time.deltaTime*170f);
						GetComponent<TP_Controller>().YRotation-=Time.deltaTime*170f;
					}


				} else {
					isHidden = true;
					foreach(MouseLook ml in GameObject.FindGameObjectWithTag("Kid").GetComponentsInChildren<MouseLook>()){
						ml.enabled=true;
					}					
				}				
			}
			
			if(time>timeToHide+timeToRotate){
				//if nun is close
				//if(reduceFieldOfView) transform.LookAt(GameObject.FindGameObjectWithTag("LookAt").transform);			

				//if(Input.GetButtonDown("Interaction") || Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0){
				if(Input.GetButtonDown("Interaction")){
					ComeOut();
				}
			}
						
		} else if(hiding && comingOut){
			if(time<timeToHide*2f/3f){
				time+=Time.deltaTime;							
				MoveCamera(gameObject.transform.position,new Vector3(outPoint.position.x,gameObject.transform.position.y,outPoint.position.z),false,timeToHide*2f/3f);
				justChanged=true;			
				
				transform.rotation = new Quaternion(transform.rotation.x-baseRotation.x/(timeToHide*2f/3f)*Time.deltaTime,transform.rotation.y,transform.rotation.z-baseRotation.z/(timeToHide*2f/3f)*Time.deltaTime,transform.rotation.w);				
				GetComponent<TP_Controller>().restoreNormalRotation((timeToHide*2f/3f));
				
			} else if(time<timeToHide){
				if(justChanged){
					firstTime=true;
					justChanged=false;
				}
				time+=Time.deltaTime;							
				MoveCamera(gameObject.transform,outPoint,false,timeToHide/3f);				
			} else {
				GetComponent<TP_Controller>().returnControl();
				firstTime=false;
				hiding=false;
				comingOut=false;
				
				Color c = hidingSpot.gameObject.transform.parent.renderer.material.color;
				hidingSpot.gameObject.transform.parent.renderer.material.color = new Color(c.r,c.g,c.b,1.0f);
				transform.rotation = new Quaternion(0f,transform.rotation.y,0f,transform.rotation.w);
			}
		}
	}
	
	public void Hide(Transform hidingPoint, Transform outPoint, HidingSpot hidingSpot){
		normalFieldOfView = Camera.mainCamera.fieldOfView;
		time = 0f;
		hiding=true;
		firstTime=true;
		this.hidingPoint=hidingPoint;
		this.hidingSpot=hidingSpot;
		GetComponent<TP_Controller>().hasControl=false;
		foreach(MouseLook ml in GameObject.FindGameObjectWithTag("Kid").GetComponentsInChildren<MouseLook>()){
			ml.enabled=false;
		}
		this.outPoint = outPoint;
		
		Color c = hidingSpot.gameObject.transform.parent.renderer.material.color;
		hidingSpot.gameObject.transform.parent.renderer.material.color = new Color(c.r,c.g,c.b,0.7f);
	}
	
	public List<Vector3> GetHidingSpotsInRange()
	{
		List<Vector3> hidingSpots = new List<Vector3>();
		RaycastHit hitInfo;
		//disable current hiding spot collider
		hidingSpot.transform.parent.GetComponent<BoxCollider>().enabled = false;
		
		int layerMask = 1 << LayerMask.NameToLayer("HidingSpots");
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, hidingSpotsRange, layerMask);
		
		layerMask += 1 << LayerMask.NameToLayer("Wall");
		foreach(Collider hit in hitColliders)
		{
			if(hit != hidingSpot.transform.parent.collider)
			{
				if(Physics.Raycast(transform.position, hit.transform.position - transform.position,
					out hitInfo, hidingSpotsRange, layerMask))
				{
					if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("HidingSpots") 
						&& hitInfo.collider == hit)
						hidingSpots.Add(hitInfo.collider.gameObject.GetComponentInChildren<OutPoint>().transform.position);
				}
			}	
		}
		//hidingSpots.Add(GetCurrentHidingSpot());
		hidingSpot.transform.parent.collider.enabled = true;
		return hidingSpots;
	}
	
	public Vector3 GetCurrentHidingSpot()
	{
		return 	hidingSpot.transform.parent.GetComponentInChildren<OutPoint>().transform.position;
	}
	
	public void ComeOut()
	{
		//If there's a nun going around, cannot come out
		if(NunAlertManager.getInstance().nunsChasing.Count>0) return;
		
		//Come out
		if(comingOut == false)
		{
			isHidden = false;
			time=0f;
			comingOut=true;
			foreach(MouseLook ml in GameObject.FindGameObjectWithTag("Kid").GetComponentsInChildren<MouseLook>()){
				ml.enabled=false;
			}
		}
	}
}
