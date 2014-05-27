using UnityEngine;
using System.Collections;

public class Headbobber : MonoBehaviour{
	 public float timer = 0.0f; 
	 public float bobbingSpeed = 0.18f; 
	 public float bobbingAmount = 0.2f; 
	 public float midpoint = 2.0f; 
	
	 private float waveslice=0f;
	 private float horizontal;
	 private float vertical;
	 private float translateChange;
	 private float totalAxes;
	
	 public bool xAxis = false;	 
	 public bool moveWhileStanding=false;
	 private SneakWalkRunController sneak;
	 private HidingController hidingController;
	
	 public float lowerMidpointLimit=-1f;
	 public float goingUpDownRatio=1.5f;
	
	 private float sneakingMultiplier=0f;
	 private float runningMultiplier=0f;
	 private float sneakingAmountMultiplier=0f;
	 private float runningAmountMultiplier=0f;
	 
	 private float updatedMidPoint;

	void Start(){
		if(transform.parent!=null) sneak = transform.parent.GetComponent<SneakWalkRunController>();
		updatedMidPoint=midpoint;
		hidingController = GameObject.FindGameObjectWithTag("Kid").GetComponent<HidingController>();
		if(sneak!=null) sneakingMultiplier = sneak.sneakingMultiplier;
		if(sneak!=null) runningMultiplier = sneak.runningMultiplier;
		if(sneak!=null) sneakingAmountMultiplier = sneak.sneakingMultiplier;
		if(sneak!=null) runningAmountMultiplier=sneak.runningMultiplier;

		runningMultiplier = 1.3f;
		runningAmountMultiplier = 1.3f;
	 }

	public Vector3 HeadBobbing(Vector3 cameraPosition){
		if(((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f)) && !hidingController.isHiddenOrHidingOrComingOut()){
			updatedMidPoint=Mathf.Clamp(updatedMidPoint-goingUpDownRatio*Time.deltaTime,lowerMidpointLimit,midpoint);
		} else {
			updatedMidPoint=Mathf.Clamp(updatedMidPoint+goingUpDownRatio*Time.deltaTime,lowerMidpointLimit,midpoint);
		}
		
		waveslice = 0.0f; 
		horizontal = Input.GetAxis("Horizontal"); 
		vertical = Input.GetAxis("Vertical"); 
		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0 && !moveWhileStanding) { 
			timer = 0.0f; 
		} 
		else { 
			waveslice = Mathf.Sin(timer);
			if(!moveWhileStanding) timer = timer + ((Input.GetButton("Run") || Input.GetAxis("Run")>0.5f)?bobbingSpeed*runningMultiplier:((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f)?bobbingSpeed*sneakingMultiplier:bobbingSpeed)); 
			else timer += bobbingSpeed;
			
			if (timer > Mathf.PI * 2) { 
				timer = timer - (Mathf.PI * 2); 
			} 
		} 
		if (waveslice != 0) { 		
			if(sneak!=null)
				translateChange = waveslice * ((Input.GetButton("Run") || Input.GetAxis("Run")>0.5f)?bobbingAmount*runningAmountMultiplier:((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f)?bobbingAmount*sneakingAmountMultiplier:bobbingAmount));
			else 
				translateChange = waveslice * (bobbingAmount);
			
			totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f); 
			if(totalAxes==0 && moveWhileStanding) totalAxes=0.2f;
			translateChange = totalAxes * translateChange;			
			float sumX = (xAxis?(midpoint + translateChange):(cameraPosition.x));
			float sumY = (xAxis?(cameraPosition.y):(updatedMidPoint + translateChange));
			cameraPosition = new Vector3(sumX,sumY,cameraPosition.z); 
		} 
		else { 
			cameraPosition = new Vector3(cameraPosition.x,updatedMidPoint,cameraPosition.z); 
		} 
		return cameraPosition;
	}

	 void Update () { 		
		
		if((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f) && !hidingController.isHiddenOrHidingOrComingOut()){
			updatedMidPoint=Mathf.Clamp(updatedMidPoint-goingUpDownRatio*Time.deltaTime,lowerMidpointLimit,midpoint);
		} else {
			updatedMidPoint=Mathf.Clamp(updatedMidPoint+goingUpDownRatio*Time.deltaTime,lowerMidpointLimit,midpoint);
		}
		
	    waveslice = 0.0f; 
	    horizontal = Input.GetAxis("Horizontal"); 
	    vertical = Input.GetAxis("Vertical"); 
		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0 && !moveWhileStanding) { 
		       timer = 0.0f; 
		    } 
		    else { 
		       waveslice = Mathf.Sin(timer);
				if(!moveWhileStanding) timer = timer + ((Input.GetButton("Run") || Input.GetAxis("Run")>0.5f)?bobbingSpeed*runningMultiplier:((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f)?bobbingSpeed*sneakingMultiplier:bobbingSpeed)); 
				else timer += bobbingSpeed;
		       
		       if (timer > Mathf.PI * 2) { 
		          timer = timer - (Mathf.PI * 2); 
		       } 
		    } 
	    if (waveslice != 0) { 		
			if(sneak!=null)
	       		translateChange = waveslice * ((Input.GetButton("Run") || Input.GetAxis("Run")>0.5f)?bobbingAmount*runningAmountMultiplier:((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f)?bobbingAmount*sneakingAmountMultiplier:bobbingAmount));
			else 
				translateChange = waveslice * (bobbingAmount);
			
	       totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
	       totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f); 
		   if(totalAxes==0 && moveWhileStanding) totalAxes=0.2f;
	       translateChange = totalAxes * translateChange;			
		   float sumX = (xAxis?(midpoint + translateChange):(transform.localPosition.x));
		   float sumY = (xAxis?(transform.localPosition.y):(updatedMidPoint + translateChange));
	       transform.localPosition = new Vector3(sumX,sumY,transform.localPosition.z); 
	    } 
	    else { 
	       transform.localPosition = new Vector3(transform.localPosition.x,updatedMidPoint,transform.localPosition.z); 
	    } 
	 }
}