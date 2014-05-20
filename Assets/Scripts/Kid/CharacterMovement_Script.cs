using UnityEngine;
using System.Collections;

public class CharacterMovement_Script: MonoBehaviour {
	
	public float rotate_speed;
	public float movement_speed;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		CharacterController controller = GetComponent<CharacterController>();
		
		transform.Rotate(0,Input.GetAxis("Horizontal") * rotate_speed, 0 );
		
		Vector3 forward = transform.TransformDirection (Vector3. forward);
		float current_speed = movement_speed * Input.GetAxis ("Vertical");
		
		if(this.tag == "Kid"){
			controller.SimpleMove (forward * current_speed);
		}else if(this.tag == "Ghost"){
			controller.Move (forward * current_speed * Time.deltaTime);
		}

	}
}
