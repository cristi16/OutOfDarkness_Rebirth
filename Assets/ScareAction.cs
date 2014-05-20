using UnityEngine;
using System.Collections;

public class ScareAction : Action {
	
	private bool running=false;	
	private float time=0f;
	public float timeToLast=1.5f;
	public float speed=2.0f;	
	private GameObject player;
	private Vector3 originalPosition;
		
	
	public override void execute(){		
		running=true;
		audio.Play();
		Camera.mainCamera.GetComponent<ScareController>().Scared();				
	}
	
	void Start(){
		player = GameObject.FindGameObjectWithTag("Kid");
		originalPosition = transform.position;
	}
	
	void Update(){
		if(running){			
			time+=Time.deltaTime;
			if(time>timeToLast){ 
				Camera.mainCamera.GetComponent<ScareController>().NotScared();								
				player.transform.localRotation=Quaternion.Euler(0f,player.transform.localEulerAngles.y,0f);
				Destroy(this.gameObject);
			} else {
				//transform.LookAt(player.transform.position - new Vector3(0f,-2f,0f));
				player.transform.LookAt(transform.position+new Vector3(0f,2f,0f));
				Vector3 direction = (player.transform.position - transform.position).normalized;				
				direction.y=0f;
				transform.position+=direction*Time.deltaTime*speed;
			}
			
		} else {
			Quaternion rot =Quaternion.LookRotation(player.transform.position - transform.position - new Vector3(0f,2f,0f));			
			transform.rotation = rot;
		}
	}
}
