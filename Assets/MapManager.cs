using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {
	
	internal bool showingMap = false;
	private GUITexture map;
	public Texture mapTexture_16_9;
	public Texture mapTexture_16_10;
	public GameObject mapCamera;
	private bool isFullScreen;
	private ArrayList questList;
	private ArrayList completedQuestList;
	[RangeAttribute(0,50)]
	public int objectivesLeftOffset = 15;
	[RangeAttribute(0,50)]
	public int objectivesTopOffset = 49;
	
	// Use this for initialization
	void Start () {
		isFullScreen = Screen.fullScreen;
		questList = new ArrayList();
		map = GetComponent<GUITexture>();
		
		float aspectRatio = Screen.width / Screen.height;
		if(Mathf.Approximately(Camera.main.aspect, 16f/9f))
			map.texture = mapTexture_16_9;
		else if(Mathf.Approximately(Camera.main.aspect, 16f/10f))
			map.texture = mapTexture_16_10;
		if(map!=null) 
			map.pixelInset=ResizeGUI(map.pixelInset);	
		
		guiText.pixelOffset =  new Vector2(Screen.width * objectivesLeftOffset/100, Screen.height * objectivesTopOffset/100);
		guiText.fontSize = (int) ( Screen.width * 35f / 960f );
		UpdateQuestList();
	}
	

	
	// Update is called once per frame
	void Update () {
		if(isFullScreen != Screen.fullScreen)
		{
			isFullScreen = Screen.fullScreen;
			map.pixelInset = new Rect(-Screen.width/2, -Screen.height/2, Screen.width, Screen.height);
			
			guiText.pixelOffset =  new Vector2(Screen.width * objectivesLeftOffset/100, Screen.height * objectivesTopOffset/100);
		}			
		
	}
	
	public void ShowMap()
	{
		map.enabled=true;
		guiText.enabled = true;
		mapCamera.SetActive(true);
		showingMap=true;	
	}
	
	public void HideMap()
	{
		map.enabled=false;
		guiText.enabled = false;
		mapCamera.SetActive(false);
		showingMap=false;	
	}
	
	private Rect ResizeGUI(Rect _rect)
	{
//	    float FilScreenWidth = _rect.width / map.texture.width;
//	    float rectWidth = FilScreenWidth * Screen.width;
//	    float FilScreenHeight = _rect.height / map.texture.height;
//	    float rectHeight = FilScreenHeight * Screen.height;
//	    float rectX = (_rect.x / map.texture.width) * Screen.width;
//	    float rectY = (_rect.y / map.texture.height) * Screen.height;
		
//		guiText.text = Screen.width + "*" + Screen.height + "\n";
//		guiText.text += "x: " + rectX + " y: " + rectY + " w: " + rectWidth + " h: " + rectHeight;
	 
	    return new Rect( -Screen.width / 2, -Screen.height / 2, Screen.width, Screen.height);
	}
	
	public void AddQuest(string questID, string quest)
	{
		LevelState.getInstance().AddQuest(questID, quest);
		UpdateQuestList();
	}
	
	public bool ContainsQuest(string questID)
	{
		return LevelState.getInstance().ContainsQuest(questID);
	}
	
	public bool ContainsCompletedQuest(string questID)
	{
		return LevelState.getInstance().ContainsCompletedQuest(questID);
	}		
	
	public void CompleteQuest(string questID)
	{
		LevelState.getInstance().CompleteQuest(questID);
		UpdateQuestList();
	}
	
	public void RemoveQuest(string questID)
	{
		LevelState.getInstance().RemoveQuest(questID);
		UpdateQuestList();
	}
	
	public void UpdateQuestList()
	{
		questList = LevelState.getInstance().GetQuestList();
		completedQuestList = LevelState.getInstance().GetCompletedQuestList();
		
		guiText.text = "Objectives:\n";
		
		for(int i=0; i<questList.Count; i++)
		{
			guiText.text += "- " + questList[i] + "\n";
		}
		
		guiText.text += "\n\nCompleted Objectives:\n";
		
		for(int i=0; i<completedQuestList.Count; i++)
		{
			guiText.text += "- " + completedQuestList[i] + "\n";
		}
		
	}
}
