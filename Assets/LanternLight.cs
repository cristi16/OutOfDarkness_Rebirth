using UnityEngine;
using System.Collections;

public class LanternLight : MonoBehaviour {
	
	private Light lantern;
	
	public float lightPercentage=0.3f;
	public float anglePercentage=0.01f;
	
	private float midLight;
	private float midAngle;	
	
	private bool lightGoingUp=true;
	private bool angleGoingUp=false;
	private bool colorGoingUp=true;
	
	private float lightInterval;
	private float angleInterval;

	private float originalMidAngle;
	
	// Use this for initialization
	void Start () {
		lantern = GetComponent<Light>();
		
		midLight = lantern.GetComponent<Light>().intensity;
		midAngle = lantern.GetComponent<Light>().spotAngle;		
		originalMidAngle = midAngle;


		CalculatePercentages ();
	}

	public void CalculatePercentages(){
		lightInterval = (midLight*(1+lightPercentage)- midLight*(1-lightPercentage))/1f;
		angleInterval = (midAngle*(1+anglePercentage)- midAngle*(1-anglePercentage))/0.4f;
	}

	public void LowMidangle(){
		if (midAngle == originalMidAngle) {
			midAngle = originalMidAngle / 3f;
			GetComponent<Light>().spotAngle = midAngle;
			CalculatePercentages ();
		}
	}

	public void NormalMidangle(){
		if (midAngle != originalMidAngle) {
			midAngle = originalMidAngle;
			GetComponent<Light>().spotAngle = midAngle;
			CalculatePercentages ();
		}
	}

	// Update is called once per frame
	void Update () {
		if(lightGoingUp){
			lantern.GetComponent<Light>().intensity+=lightInterval * Time.deltaTime;
			if(lantern.GetComponent<Light>().intensity>midLight*(1+lightPercentage)) lightGoingUp=false;
		} else {
			lantern.GetComponent<Light>().intensity-=lightInterval * Time.deltaTime;
			if(lantern.GetComponent<Light>().intensity<midLight*(1-lightPercentage)) lightGoingUp=true;
		}
		if(angleGoingUp){
			lantern.GetComponent<Light>().spotAngle+=angleInterval * Time.deltaTime;
			if(lantern.GetComponent<Light>().spotAngle>midAngle*(1+anglePercentage*2)) angleGoingUp=false;
		} else {
			lantern.GetComponent<Light>().spotAngle-=angleInterval * Time.deltaTime;
			if(lantern.GetComponent<Light>().spotAngle<midAngle*(1-anglePercentage*2)) angleGoingUp=true;
		}		
	}
}
