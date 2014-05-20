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
	
	// Use this for initialization
	void Start () {
		lantern = GetComponent<Light>();
		
		midLight = lantern.light.intensity;
		midAngle = lantern.light.spotAngle;		
		
		lightInterval = (midLight*(1+lightPercentage)- midLight*(1-lightPercentage))/1f;
		angleInterval = (midAngle*(1+anglePercentage)- midAngle*(1-anglePercentage))/0.4f;
	}
	
	// Update is called once per frame
	void Update () {
		if(lightGoingUp){
			lantern.light.intensity+=lightInterval * Time.deltaTime;
			if(lantern.light.intensity>midLight*(1+lightPercentage)) lightGoingUp=false;
		} else {
			lantern.light.intensity-=lightInterval * Time.deltaTime;
			if(lantern.light.intensity<midLight*(1-lightPercentage)) lightGoingUp=true;
		}
		if(angleGoingUp){
			lantern.light.spotAngle+=angleInterval * Time.deltaTime;
			if(lantern.light.spotAngle>midAngle*(1+anglePercentage*2)) angleGoingUp=false;
		} else {
			lantern.light.spotAngle-=angleInterval * Time.deltaTime;
			if(lantern.light.spotAngle<midAngle*(1-anglePercentage*2)) angleGoingUp=true;
		}		
	}
}
