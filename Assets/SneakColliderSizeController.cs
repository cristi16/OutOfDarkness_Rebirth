using UnityEngine;
using System.Collections;

public class SneakColliderSizeController : MonoBehaviour {
	private SneakWalkRunController sneak;

	public float sneakingColliderHeight=1f;
	public float normalColliderHeight=4.25f;

	public float sneakingColliderCenter=2f;
	public float normalColliderCenter=0.57f;

	public float normalY=0.24f;

	private CharacterController character;

	// Use this for initialization
	void Start () {
		sneak = GameObject.FindGameObjectWithTag("Kid").GetComponent<SneakWalkRunController>();
		character = GameObject.FindGameObjectWithTag ("Kid").GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(sneak.getSneak() && ((Input.GetButton("Sneak") || Input.GetAxis("Sneak")>0.5f))){
			character.height=sneakingColliderHeight;
			character.center = new Vector3(character.center.x,sneakingColliderCenter,character.center.z);

			//updatedMidPoint=Mathf.Clamp(updatedMidPoint-goingUpDownRatio*Time.deltaTime,lowerMidpointLimit,midpoint);
		} else {
			character.height=normalColliderHeight;
			character.center = new Vector3(character.center.x,normalColliderCenter,character.center.z);
			character.transform.position=new Vector3(character.transform.position.x,normalY,character.transform.position.z);
			//updatedMidPoint=Mathf.Clamp(updatedMidPoint+goingUpDownRatio*Time.deltaTime,lowerMidpointLimit,midpoint);
		}
	}
}
