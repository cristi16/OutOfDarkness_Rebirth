using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Keys{KeyToServiceStairs}

public class LevelState : MonoBehaviour {
	
	internal static bool created = false;
	private Hashtable initiallyLockedDoors;
	private Hashtable hasKeyDoors;
	private Hashtable amuletDoors;
	private Hashtable usedKeys;
	private Hashtable savedKeys;
	private Hashtable checkpoints;	
	private Hashtable collectibles;
	private Hashtable quests;
	private Hashtable completedQuests;
	
	private ArrayList questOrder;
	private ArrayList completedQuestOrder;
	
	private bool firstLoad = true;
	private int lastCheckpoint = -1;	
	internal bool playIntro;
	public List<HelpKeys> feedbackShown;
	private HashSet<string> losses;		
	public bool checkpointsActivateAutomatically=false;
	public bool showCursor = false;	
	internal bool usingMainMenu = true;
	
	public bool flashlightActivated = true;
	public bool mapActivated = true;
	public bool sneakActivated = true;
	public bool runActivated = true;
	internal bool shownMoveFeedback=false;	
	public bool foundClueToKey=false;
	public int rhymesFound=0;	
	public bool diddleSolved=false;
	public bool ignorePreviousLevelState=false;
	public bool keyToServiceStairs=false;
		
	internal List<Keys> pickedUpKeys=new List<Keys>();
	
	internal bool inPlay;
	
	private static LevelState instance;
	internal static bool mainMenuOrder=true;
	internal static bool newScene=false;
	
	internal bool puzzleMode=false;
	internal bool started=false;
	
	private HashSet<string> checkedDoors;

	internal int resolution;

	public static LevelState getInstance(){		
		return instance;
	}			
	
	void Update(){
		if(newScene){
			newScene=false;
			LoadScene();
		}
		
		if(inPlay && !showCursor && !puzzleMode){			
			Screen.lockCursor=true;
		}
		//if(puzzleMode) Screen.lockCursor=false;
	}
	
	void Awake() 
	{
	    if (!created)
		{
	        // this is the first instance - make it persist
	        DontDestroyOnLoad(this.gameObject);
	        created = true;
			instance = this;
	    } else {
	        // this must be a duplicate from a scene reload - DESTROY!
	        Destroy(this.gameObject);
    	} 
	}
	// Use this for initialization
	void Start () 
	{
		started=true;
		Screen.showCursor = showCursor;

		resolution = Mathf.CeilToInt (Screen.width * 0.94f);

		initiallyLockedDoors = new Hashtable();
		hasKeyDoors = new Hashtable();
		amuletDoors = new Hashtable();
		checkpoints = new Hashtable();
		savedKeys = new Hashtable();
		usedKeys = new Hashtable();
		collectibles = new Hashtable();
		quests = new Hashtable();
		completedQuests = new Hashtable();
		questOrder = new ArrayList();
		completedQuestOrder = new ArrayList();
		losses = new HashSet<string>();
		feedbackShown = new List<HelpKeys>();
		checkedDoors = new HashSet<string>();		
		
		LoadScene();	
		if(ignorePreviousLevelState){ 
			usingMainMenu=true;
			GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuManager>().showMainMenu=true;
		}
		
		GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>().AddQuest("escape","Escape from the Orphanage.");	
	}
	
	private void SaveDoorsState()
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject door in doors)
		{
			initiallyLockedDoors[door.transform.parent.name] = door.transform.parent.GetComponentInChildren<DoorInteraction>().initiallyLocked;
			hasKeyDoors[door.transform.parent.name] = door.transform.parent.GetComponentInChildren<DoorInteraction>().hasKey;	
			amuletDoors[door.transform.parent.name] = door.transform.parent.GetComponentInChildren<DoorInteraction>().hasAmulet;	
			usedKeys[door.transform.parent.name] = door.transform.parent.GetComponentInChildren<DoorInteraction>().usedKey;
		}
	}
	
	private void LoadDoorStates()
	{
		GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
		foreach(GameObject door in doors)
		{
			door.transform.parent.GetComponentInChildren<DoorInteraction>().initiallyLocked = (bool) initiallyLockedDoors[door.transform.parent.name];
			door.transform.parent.GetComponentInChildren<DoorInteraction>().hasKey = (bool) hasKeyDoors[door.transform.parent.name];
			door.transform.parent.GetComponentInChildren<DoorInteraction>().hasAmulet = (bool) amuletDoors[door.transform.parent.name];	
			door.transform.parent.GetComponentInChildren<DoorInteraction>().usedKey = (bool) usedKeys[door.transform.parent.name];	
		}		
	}
	
	public bool AddCheckedDoor(string doorName){
		return checkedDoors.Add(Application.loadedLevel + doorName);
	}
	
	public bool ContainsCheckedDoor(string doorName){
		return checkedDoors.Contains(Application.loadedLevel + doorName);
	}
	
	private void SaveKeys()
	{
		GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
		foreach(GameObject key in keys)
		{
			savedKeys[key.name] = key.GetComponentInChildren<Renderer>().enabled;	
		}
	}
	
	public bool HasLost(NunStateMachine nun){
		if(losses!=null && losses.Contains(nun.name)) return true;
		return false;
	}
	
	public void SaveLoss(NunStateMachine nun)
	{				
		losses.Add(nun.name);		
	}
	
	private void LoadKeys()
	{
		GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
		foreach(GameObject key in keys)
		{
			key.GetComponentInChildren<Renderer>().enabled = (bool)savedKeys[key.name];
			key.GetComponentInChildren<Collider>().enabled = (bool)savedKeys[key.name];;
			//key.GetComponentInChildren<Light>().enabled = (bool)savedKeys[key.name];;
		}
	}
	
	public void SaveCollectible(string key, bool collected)
	{
		collectibles[key] = collected;
	}
	
	public bool IsCollected(string key)
	{
		if(collectibles == null || collectibles[key] == null) return false;
		
		return (bool)collectibles[key];
	}
	
	private void SaveCheckPoints()
	{
		GameObject temp = GameObject.Find("Game Manager");
		CheckpointsManager_Script manager = temp.GetComponent<CheckpointsManager_Script>();
		
		foreach(Checkpoint_Script check in manager.checkpoints)
		{
			checkpoints[check.gameObject.name] = check.activated;
		}
		
		lastCheckpoint = manager.lastCheckpoint;		
		
	}
	
	private void LoadCheckPoints()
	{
		GameObject temp = GameObject.Find("Game Manager");
		CheckpointsManager_Script manager = temp.GetComponent<CheckpointsManager_Script>();
		
		foreach(Checkpoint_Script check in manager.checkpoints)
		{
			check.activated =  (bool) checkpoints[check.gameObject.name];
		}		
		manager.lastCheckpoint = lastCheckpoint;
		
	}
	
	public void SaveData()
	{
		//SaveDoorsState();
		//SaveKeys();		
		SaveCheckPoints();	
	}
	
	public void LoadScene(){		
		if(mapActivated){ 
			GameObject o = GameObject.FindGameObjectWithTag("MapActivated");	
			if(o!=null) DestroyInteractiveElements(o);			
		}
		
		if(flashlightActivated){ 			
			GameObject o = GameObject.FindGameObjectWithTag("FlashlightActivated");
			if(o!=null){
				DestroyInteractiveElements(o);
				o.GetComponent<ColliderEnabler>().Enable();
				o.GetComponent<GameObjectDestroyer>().Destroy();			
			}
			GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>().EnableDisableFlashlights();			
		}						
		
		if(keyToServiceStairs && GameObject.FindGameObjectWithTag("KeyServiceStairs") != null){
			GameObject.FindGameObjectWithTag("KeyServiceStairs").GetComponentInChildren<Key_Script>().ActivateKey();
			GameObject.Destroy(GameObject.FindGameObjectWithTag("KeyServiceStairs"),2.0f);
		}
		
		if(foundClueToKey){
			GameObject o = GameObject.FindGameObjectWithTag("FoundClueToKey");
			if(o!=null){
				Destroy(o.GetComponentInChildren<TextTrigger>());
				Destroy(o.GetComponentInChildren<HelpTriggerEnabler>());
				//DestroyInteractiveElements(o);			
			}
		}
		
		if(diddleSolved){
			GameObject[] disappear = GameObject.FindGameObjectsWithTag("Diddle");
			GameObject appear = GameObject.FindGameObjectWithTag("NoDiddle");
			
			if(disappear!=null && appear!=null){
				foreach(GameObject g in disappear) Destroy(g);
				appear.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
		
		if(!mainMenuOrder && GameObject.FindGameObjectWithTag("IntroCamera")!=null) GameObject.FindGameObjectWithTag("IntroCamera").GetComponent<TutorialStartCamera>().isActivated=true;
	}
	
	public void NewScene(){
		lastCheckpoint=-1;
		checkpoints = new Hashtable();
	}
	
	public void LoadSavedData()
	{
		if(firstLoad)
		{
			firstLoad = false;
			return;
		}
		Destroy(GameObject.FindGameObjectWithTag("Music"));
		//if(initiallyLockedDoors!=null && initiallyLockedDoors.Count > 0)
			//LoadDoorStates();
		//if(savedKeys.Count > 0)
			//LoadKeys();				
		
		if(checkpoints.Count > 0)
		{
			LoadCheckPoints();
		}
			
		GameObject temp = GameObject.Find("Game Manager");
		CheckpointsManager_Script manager = temp.GetComponent<CheckpointsManager_Script>();
		temp.GetComponent<MenuManager>().isMainMenu = usingMainMenu;
		
		TP_Controller controller = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
		controller.returnControl();
		
		LoadScene();
		
		
	}		
	
	private void DestroyInteractiveElements(GameObject o){
		InteractiveCollider c = o.GetComponent<InteractiveCollider>();
		if(c==null) c = o.GetComponentInChildren<InteractiveCollider>();
		
		InteractiveTrigger t = o.GetComponent<InteractiveTrigger>();
		if(t==null) t = o.GetComponentInChildren<InteractiveTrigger>();
		
		Destroy(t);
		Destroy(c);
	}
	
	public void ClearLevelState(){
		initiallyLockedDoors.Clear();
		hasKeyDoors.Clear();
		amuletDoors.Clear();
		savedKeys.Clear();
		checkpoints.Clear();
		losses.Clear();
		collectibles.Clear();
		firstLoad = true;
	}
	
	public void RemoveKeyIfSaved(string keyName)
	{
		if(savedKeys.ContainsKey(keyName))
			savedKeys.Remove(keyName);
	}
	
	public bool ContainsCompletedQuest(string questID)
	{
		return completedQuests.Contains(questID);
	}
	
	public bool ContainsQuest(string questID)
	{
		return quests.ContainsKey(questID);
	}
	
	public void AddQuest(string questID, string quest)
	{
		quests.Add(questID, quest);
		questOrder.Add(questID);
	}
	
	public string CompleteQuest(string questID){		
		completedQuests.Add(questID,(string)quests[questID]);
		completedQuestOrder.Add(questID);
		
		return RemoveQuest(questID);		
	}
	
	public string RemoveQuest(string questID)
	{
		string quest = (string)quests[questID];
		quests.Remove(questID);
		questOrder.Remove(questID);
		
		return quest;
	}
	
	public ArrayList GetQuestList()
	{
		ArrayList questList = new ArrayList();
		
		if(questOrder==null) return new ArrayList();
			
		for(int i = 0; i < questOrder.Count; i++)
			questList.Add((string)quests[questOrder[i]]);
		
		return questList;
	}
	
	public ArrayList GetCompletedQuestList()
	{			
		ArrayList questList = new ArrayList();
		
		if(completedQuestOrder==null) return new ArrayList();
		
		for(int i = 0; i < completedQuestOrder.Count; i++)
			questList.Add((string)completedQuests[completedQuestOrder[i]]);
		
		return questList;
	}

}
