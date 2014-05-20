using UnityEngine;
using System.Collections;

public class BlowingMechanic_Script : MonoBehaviour {

	
	public float power;
	private RaycastHit hit;
	
	private AudioSource audioSource;
	private AudioClip sound;
	private AudioManager am;
	
	// Use this for initialization
	void Start () {
		am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		sound = am.ghostInteract;
		audioSource = GetComponent<AudioSource>();
	}
	
	void OnTriggerStay(Collider col){
		// if the kid is in control we don't interact with the ghost
		if(SwitchMechanic_Script.getKidControl()) return;
		
		Debug.DrawRay(transform.parent.position, col.transform.position-transform.parent.position, Color.blue);
		
		if(Input.GetButtonDown("Interaction")){
			
			//transform.parent.FindChild("Blowing Particles").gameObject.
			
			if(col.CompareTag("Activable")){
				Distraction temp = col.GetComponent<Distraction>();
				if( temp!= null){
					temp.distractNunsArray();
				}
				
				Scare temp2 = col.GetComponent<Scare>();
				if( temp2 != null){
					temp2.scareNunsArray();
				}
				if(!audioSource.isPlaying) audioSource.PlayOneShot(sound);
			
			}
			
			if(col.CompareTag("Curtain"))
			{
				SlideCurtains curtain = col.transform.parent.GetComponent<SlideCurtains>();
				if(curtain.opened == false)
					curtain.slideCurtains = true;
				
				if(!audioSource.isPlaying) audioSource.PlayOneShot(sound);
			
			}
			if(col.CompareTag("Blowable")){
				if(Physics.Raycast(transform.parent.position, col.transform.position-transform.parent.position,out hit)){ //IF the object are tag movable
					if(hit.transform.CompareTag("Blowable"))
						hit.rigidbody.AddForce(Vector3.Normalize(col.transform.position-transform.parent.position) * power * (col.transform.position-transform.parent.position).magnitude);
				}
			}
		}
	}
}


