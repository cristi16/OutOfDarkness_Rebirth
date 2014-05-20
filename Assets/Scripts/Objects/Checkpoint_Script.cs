using UnityEngine;
using System.Collections;

public class Checkpoint_Script : MonoBehaviour {

	private CheckpointsManager_Script manager;
	private NunAlertManager nunManager;
	public bool activated = false;
	private ParticleSystem particles;
	public bool checkpointActivatesAutomatically=true;
	
	private InteractiveTrigger trigger;
	private InteractiveCollider collider;

	private SavingGUI savingGUI;
	
	// Use this for initialization
	void Start () {
		savingGUI = GameObject.FindGameObjectWithTag("CheckpointGUI").GetComponent<SavingGUI>();
		GameObject temp = GameObject.Find("Game Manager");
		if (temp != null)
			manager = temp.GetComponent<CheckpointsManager_Script>();
		
		nunManager = temp.GetComponent<NunAlertManager>();
		particles = GetComponentInChildren<ParticleSystem>();
		
		if(activated && particles != null){
			particles.Stop();
		}
		
		if(activated){
			if(renderer!=null) renderer.material = manager.openVentMaterial;
		}
		
		trigger = GetComponent<InteractiveTrigger>();
		collider = GetComponent<InteractiveCollider>();
				
		//transform.position = new Vector3(transform.position.x,GameObject.FindGameObjectWithTag("Kid").transform.position.y+0.1f,transform.position.z);
		//checkpointsActivateAutomatically = LevelState.getInstance().checkpointsActivateAutomatically;
	}
	
	void Update()
	{
		if(activated && particles != null){
			particles.Stop();
		}
	
	}
	
	void OnTriggerStay(Collider col)
	{		
		if(col.CompareTag("Kid") && (checkpointActivatesAutomatically || (trigger!=null && trigger.getGui()) || (collider!=null && collider.activateHelpCondition())))
		{
			// if the nuns are chasing the kid she won't be able to activate the checkpoint
			if(nunManager.nunsChasing.Count > 0) return;
			
			if(!activated)
			{
				activated = true;
				manager.pushCheckpoint(this);
				//if(!checkpointActivatesAutomatically) renderer.material = manager.openVentMaterial;
				//savingGUI.ShowSavingText();
				//if(particles != null && particles.isPlaying) particles.Stop();
			}
		}
	}
}
