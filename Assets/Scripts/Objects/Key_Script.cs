using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Key_Script : MonoBehaviour {
	
	/// A list of doors that will
	public List<GameObject> doors;
	public Texture2D key_icon;
	public int icon_dimenstion = 50;
	
	private AudioSource audioSource;	
	private AudioManager am;	
	private KeyBehaviour keyBehaviour;
	
	public Keys key;
		
	// Use this for initialization
	void Start () {
		am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();		
		audioSource = GetComponent<AudioSource>();
		keyBehaviour = GetComponent<KeyBehaviour>();
		
		if(LevelState.getInstance().pickedUpKeys.Contains(key)){
			keyBehaviour.ExecuteAction(true);
			ActivateKey();
			Destroy(transform.parent.gameObject);
		}		
	}
	
	void Update(){		
	}
	
	public void ActivateKey(){		
		foreach(GameObject door in doors)
		{
			door.GetComponentInChildren<DoorInteraction>().kidUnlock(); //Something happens
		}
		LevelState.getInstance().pickedUpKeys.Add(key);
	}
	
	void OnTriggerStay(Collider col){
		
		/*if(col.CompareTag("Kid")){
			if( Input.GetButton("Interaction") )
			{	
				ActivateKey();
				
				//Destroy(transform.parent.gameObject);
				this.transform.parent.GetComponentInChildren<Renderer>().enabled = false;
				this.transform.parent.GetComponentInChildren<Collider>().enabled = false;
				this.transform.parent.GetComponentInChildren<Light>().enabled = false;
				this.transform.parent.GetComponentInChildren<ParticleSystem>().Stop();
			}
		}*/
	}

	void OnGUI () {
		
		if(doors[0].GetComponentInChildren<DoorInteraction>().hasKey == false || doors[0].GetComponentInChildren<DoorInteraction>().usedKey == true) return;
		
		//GUI.Label(new Rect (10,10, icon_dimenstion, icon_dimenstion), key_icon);
		
	}
}
