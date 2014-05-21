using UnityEngine;
using System.Collections;

public class InteractiveCollider : MonoBehaviour {

	internal bool inRange=false;
	internal bool mouseOver=false;	
	private float cursorSizeX=70f;
	private float cursorSizeY=70f;
	private MenuManager menuManager;	
	private Transform player;
	internal bool showingInteractiveObject;
	internal float yOffset;	
	private float distanceRatio=1.4f;
	internal bool hidingObject=false;
	private HidingController hc;
	private MapManager mapManager;

	
	// Use this for initialization
	void Start () {
		menuManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuManager>();
		player = GameObject.FindGameObjectWithTag("Kid").transform;		
		hidingObject = (GetComponentInChildren<HidingPoint>()!=null);
		hc = player.GetComponent<HidingController>();
		mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
	}

	// Update is called once per frame
	void Update () {	
		mouseOver = false;
	}

	void OnMouseEnter(){		
		if(inRange){
			mouseOver=true;
		}
	}
	
	void OnMouseOver(){
		if (inRange) {
			mouseOver = true;
		}
	}
	
	void OnMouseExit(){
		mouseOver=false;
	}
	
	public bool activateHelpCondition(){
		return inRange && mouseOver;
	}
	
	void OnGUI(){
	    if(inRange && mouseOver && !showingInteractiveObject && !mapManager.showingMap){			
			float distance = Mathf.Clamp(Vector3.Distance(transform.position,player.position),-4f,4f) + 1f;			
			
			if(hidingObject){
				distance-=2f;	
				if(hc.hiding || hc.comingOut){
					return;
				}
			}
			
			yOffset = Mathf.Clamp(-player.transform.position.y - transform.position.y,-4f,4f);
			if(TP_Motor.oculusRift){

				GUI.DrawTexture (new Rect(-Screen.width/4+Input.mousePosition.x + cursorSizeX, (Screen.height-Input.mousePosition.y)-cursorSizeY/2 + cursorSizeY/2 + yOffset*Screen.height/100f, cursorSizeX*(distanceRatio/distance), cursorSizeY*(distanceRatio/distance)),hidingObject?menuManager.hidingCursor:menuManager.cursor);
				GUI.DrawTexture (new Rect(Screen.width/4 + Input.mousePosition.x -cursorSizeX/2f , (Screen.height-Input.mousePosition.y)-cursorSizeY/2 + cursorSizeY/2 + yOffset*Screen.height/100f, cursorSizeX*(distanceRatio/distance), cursorSizeY*(distanceRatio/distance)),hidingObject?menuManager.hidingCursor:menuManager.cursor);

			} else {
				GUI.DrawTexture (new Rect(Input.mousePosition.x-cursorSizeX/2 + cursorSizeX/2, (Screen.height-Input.mousePosition.y)-cursorSizeY/2 + cursorSizeY/2 + yOffset*Screen.height/100f, cursorSizeX*(distanceRatio/distance), cursorSizeY*(distanceRatio/distance)),hidingObject?menuManager.hidingCursor:menuManager.cursor);
			}
	    }
	}
}
