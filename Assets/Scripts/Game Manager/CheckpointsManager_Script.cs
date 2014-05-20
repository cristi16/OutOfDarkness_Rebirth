using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CheckpointsManager_Script : MonoBehaviour {
	
	internal List<Checkpoint_Script> checkpoints;
	private bool checkpoint_gui;
	private SwitchMechanic_Script switchMechanic;
	public Material openVentMaterial;
	
	private GameObject kid;	
	internal float kidRadius;
	
	public Transform kidStart;	
	internal bool resetLevel = false;
	private float timer = 0f;
	private float timeUntilResetEnds = 1f;
	private SceneFadeInOut fader;	
	public float kidSpawnOffset = 7f;
	public bool playIntroCamera = false;	
	
	public Material lockedDoorMaterial;
	public Material unlockedDoorMaterial;
	public Material unusableDoorMaterial;
	public Material lockIcon;
	public Material keyIcon;
	public Material unknownIcon;
	
	public Texture2D interactTexture;
	
	internal int lastCheckpoint = -1;	
	
	private static int CompareGameObjectsByName(GameObject x, GameObject y)
    {
             if (x == null)
        {
            if (y == null)
            {
                // If x is null and y is null, they're 
                // equal.  
                return 0;
            }
            else
            {
                // If x is null and y is not null, y 
                // is greater.  
                return -1;
            }
        }
        else
        {
            // If x is not null... 
            // 
            if (y == null)
                // ...and y is null, x is greater.
            {
                return 1;
            }
            else
            {
                // ...and y is not null, compare the  
                // lengths of the two strings. 
                // 
                return x.name.CompareTo(y.name);                
            }
        }
    }
	
	// Use this for initialization
	void Start () {
		switchMechanic = gameObject.GetComponent<SwitchMechanic_Script>();
		
		checkpoint_gui = false;
		checkpoints = new List<Checkpoint_Script>();
		GameObject[] checkpointGameObjects = GameObject.FindGameObjectsWithTag("CheckPoint");		
		
		Array.Sort(checkpointGameObjects,CompareGameObjectsByName);
		
		foreach(GameObject check in checkpointGameObjects)
		{
			checkpoints.Add(check.GetComponent<Checkpoint_Script>());
		}			
		
		kid = GameObject.FindGameObjectWithTag("Kid");		
		fader = GameObject.FindGameObjectWithTag("SceneFader").GetComponent<SceneFadeInOut>();
		kidRadius = kid.transform.lossyScale.x * kid.GetComponentInChildren<CharacterController>().radius;
		
		if(kid == null){
			Debug.LogError("Error in the initialization of ChekpointsManager_script");
			return;
		}
				
		LevelState.getInstance().LoadSavedData();	
		
		if(lastCheckpoint != -1)
		{
			Transform kidSpawn = checkpoints[lastCheckpoint].transform.FindChild("KidSpawnPoint");			
			
			kid.transform.position = kidSpawn.position;
			kid.transform.localRotation = Quaternion.LookRotation(kidSpawn.forward);			
		}
		else
		{
			if(!LevelState.getInstance().usingMainMenu)
				kid.transform.position = new Vector3(kidStart.position.x, kid.transform.position.y, kidStart.position.z);			
		}
		
		// if we are not playing the tutorial show the notes count
		//if(Application.loadedLevel != 0)
			//gameObject.GetComponent<CollectiblesManager_Script>().enabled = true;
	}
	
	void Update()
	{
		// reset level state
//		if(Input.GetKeyUp(KeyCode.P))
//		{
//			LevelState.getInstance().ClearLevelState();
//			ResetLevel();
//		}
		
	}
	
	void OnTriggerStay(Collider col) {
//		if(col.CompareTag("Kid")){
//			if(Input.GetButton("Interaction"))
//				checkpoint_gui = true;
//		}
	}
	
	void OnTriggerExit(){
		checkpoint_gui = false;
	}
	
	void OnGUI () {
		if(checkpoint_gui){
			GUI.Box(new Rect(10,10,150,300), "Checkpoint Menu");
	
			for(int i = 0; i < checkpoints.Count; i++){
				if(checkpoints[i].activated == false) continue;
				
				if(GUI.Button(new Rect(20,40+i*60,130,50), checkpoints[i].name)) {
					kid.transform.position = new Vector3(checkpoints[i].transform.position.x, 
						kid.transform.position.y, checkpoints[i].transform.position.z);					
					checkpoint_gui = false;
				}
			}
		}
	}
	
	public void pushCheckpoint(Checkpoint_Script new_checkpoint){
		checkpoints.Add(new_checkpoint);
		lastCheckpoint = checkpoints.IndexOf(new_checkpoint);		
		LevelState.getInstance().SaveData();
	}
	
	public void ResetLevel()
	{
		if(resetLevel == false)
		{
			LevelState.getInstance().usingMainMenu = gameObject.GetComponent<MenuManager>().isMainMenu;
			resetLevel = true;		
			fader.sceneEnding = true;			
		}		
	}	
	
	public void LoadTutorial(){
		Application.LoadLevel(0);
	}
	
	public void LoadTestLevel(){
		Application.LoadLevel(1);
	}
	
	
	
}
