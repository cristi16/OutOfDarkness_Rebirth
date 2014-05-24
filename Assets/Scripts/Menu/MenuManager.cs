using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	//Camera Variables
	public Camera gameCam;
	public bool isPaused = false;
	public bool isMainMenu = false;
	public bool isCredits = false;
	public bool isControls = false;
	public GUIText soundNotice;
	public bool disableIntroMusicAtStart=false;
	//public GUIText notesNotice;
	
	//GUI Settings
	public GUISkin customSkin;
	float screenW = Screen.width/2;
	float screenH = Screen.height/2;
	float boxW = 120;
	float boxH = 100;
	int buttonWidth = 200;
	public Texture logo;
	public Texture start;
	public Texture btnExit;
	public Texture creditImage;
	public Texture controlsImage;	
	
	private TutorialStartCamera tutorialCamera;	
	private SceneFadeInOut fader;
	private bool isQuiting = false;
	
	private bool controlsFromMenu=false;
	private bool controlsFromPause=false;
	
	private TP_Controller player;
	private MapManager mapManager;
	private bool hideCursor=false;
	public Texture2D cursor;
	public Texture2D hidingCursor;
	public bool showMainMenu=true;
	private bool loadingLevel=false;
	internal static bool newScene=false;
	
	// Use this for initialization
	void Start () {		
		// If you return to the menu from ingame, ensure time is running
		Time.timeScale = 1;
		
		tutorialCamera = GameObject.FindGameObjectWithTag("IntroCamera").GetComponent<TutorialStartCamera>();	
		mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
		fader = GameObject.FindGameObjectWithTag("SceneFader").GetComponent<SceneFadeInOut>();
		player = GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>();
		
		if(newScene){
			LevelState.newScene=true;
			newScene=false;
		}
		
		if(LevelState.getInstance().started) LevelState.getInstance().LoadSavedData();
		isMainMenu = LevelState.getInstance().usingMainMenu;			
	}
	
	void Update() {		
		screenW = Screen.width/2;
		screenH = Screen.height / 2;
		if(loadingLevel){ 
			soundNotice.text = "Loading... "+(int)(Application.GetStreamProgressForLevel(1)*100)+"%";
			if(Application.CanStreamedLevelBeLoaded(1)) Application.LoadLevel(1);			
		}
		
		LevelState.getInstance().inPlay=inPlay();

		if(isMainMenu && showMainMenu && Input.GetButtonDown("Interaction")){
			tutorialCamera.isActivated = true;						
			if(soundNotice!=null){
				Destroy(soundNotice.gameObject);
			}
			Screen.lockCursor = true;
			if(disableIntroMusicAtStart) GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().reduceVolumeTemp();						
			//notesNotice.enabled=true;		
		}

		if(Input.GetKeyDown(KeyCode.Escape) && !LevelState.getInstance().puzzleMode && !mapManager.showingMap)
		{
			if(!isMainMenu && !isCredits && !isControls && !LevelState.getInstance().puzzleMode){				
				if(!isPaused){
					isPaused = !isPaused;
					player.removeControl();					
				}/* else{
				    isPaused = !isPaused;
					player.returnControl();		
					hideCursor=true;
				}*/
			}
			/*
			if(isCredits)
			{
				isCredits = false;				
				isMainMenu = true;
			}
			if(isControls)
			{				
				isControls = false;
				isPaused = controlsFromPause;
				isMainMenu = controlsFromMenu;
				controlsFromPause=false;
				controlsFromMenu=false;
			}
			*/
		}				
	}
		
	public bool inPlay(){
		return !(isPaused || isMainMenu || isCredits || isControls);
	}
	
	void OnGUI() {
		if(loadingLevel) return;
		
		GUI.skin = customSkin;
		
		if(hideCursor){ 
			Screen.showCursor=false;			
			Screen.lockCursor=true;
			hideCursor=false;
		}
		
		if((isPaused || (isMainMenu && !tutorialCamera.isActivated) || isCredits || isControls)){
			Screen.lockCursor=false;
			Screen.showCursor = true;						
		}		
		
		if(isPaused){
			if(!isQuiting)
				Time.timeScale = 0.01f;
			else
			{				
				Time.timeScale = 1f;
				fader.gameEnding = true;
				float alpha = Mathf.Lerp( GUI.color.a, 0f, 1.5f * Time.deltaTime);
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			}
			GUI.DrawTexture(new Rect((screenW - 200), screenH-350, 400, 400),logo);
			//Put button images up first, then the buttons on top
			GUILayout.BeginArea (new Rect((screenW - (buttonWidth/2)), screenH,buttonWidth,260));
				GUILayout.BeginVertical();
					if(GUILayout.Button("Resume",GUILayout.Width(buttonWidth))){
						Time.timeScale = 1;
						hideCursor=true;
						player.returnControl();
						isPaused = false;
					}
					/*if(GUILayout.Button("Restart",GUILayout.Width(buttonWidth))){
						LevelState.getInstance().usingMainMenu = false;
						Application.LoadLevel(Application.loadedLevel);
					}*/
					if(GUILayout.Button("Controls",GUILayout.Width(buttonWidth))){
						isControls = true;
						controlsFromPause=true;
					}
					if(GUILayout.Button("Main Menu",GUILayout.Width(buttonWidth))){
						LevelState.created=false;
						Destroy(LevelState.getInstance().gameObject);
						LevelState.mainMenuOrder=true;
						Application.LoadLevel(0);
					}
					//Exiting the application doesn't work in web build, so disable the button
					if(!Application.isWebPlayer){
						
						if(GUILayout.Button("Quit",GUILayout.Width(buttonWidth))){
							isQuiting = true;							
							Application.Quit();	
						}
						
					}
				GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		else
		{
			Time.timeScale = 1f;
		}
		if(isMainMenu && showMainMenu){
			Time.timeScale = 1f;
			if(isQuiting)
				fader.gameEnding = true;
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, tutorialCamera.guiAlpha);
			if(!TP_Motor.oculusRift){
				GUI.DrawTexture(new Rect((screenW - 200), screenH-350, 400, 400),logo);
			} else {
				//OVRGUI.instance.StereoDrawTexture(screenW/2f,screenH/2f,300f,300f,ref logo,Color.white);
				GUI.DrawTexture(new Rect((screenW/2 - 100), screenH/4, 300, 300),logo);
				GUI.DrawTexture(new Rect((3*screenW/2 - 200), screenH/4, 300, 300),logo);

				GUI.DrawTexture(new Rect((screenW/2), screenH/4 + 250, 160, 110),start);
				GUI.DrawTexture(new Rect((3*screenW/2 - 100), screenH/4 + 250, 160, 110),start);
			}
			//Put button images up first, then the buttons on top

			/*GUILayout.BeginArea (new Rect((screenW - (buttonWidth/2)) + screenW/2, screenH/2,buttonWidth,260));
			GUILayout.BeginVertical();					
			GUILayout.Button("Start",GUILayout.Width(buttonWidth));
			GUILayout.Button("Level 2",GUILayout.Width(buttonWidth));
			GUILayout.Button("Controls",GUILayout.Width(buttonWidth));
			GUILayout.Button("Quit",GUILayout.Width(buttonWidth));
			GUILayout.EndVertical();
			GUILayout.EndArea();
			GUILayout.BeginArea (new Rect((screenW) - screenW/2, screenH/2,buttonWidth,260));
				GUILayout.BeginVertical();					
				if(GUILayout.Button("Start",GUILayout.Width(buttonWidth))){							
					tutorialCamera.isActivated = true;						
					if(soundNotice!=null){
						Destroy(soundNotice.gameObject);
					}
					Screen.lockCursor = true;
					if(disableIntroMusicAtStart) GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().reduceVolumeTemp();						
					//notesNotice.enabled=true;						
				}
				if(GUILayout.Button("Level 2",GUILayout.Width(buttonWidth))){						
					
					LevelState.created=false;
					Destroy(LevelState.getInstance().gameObject);
					LevelState.mainMenuOrder=false;
					loadingLevel=true;												
				}
				if(GUILayout.Button("Controls",GUILayout.Width(buttonWidth))){
					isControls = true;				
					controlsFromMenu=true;
				}

			//Exiting the application doesn't work in web build, so disable the button
			if(!Application.isWebPlayer){
				if(GUILayout.Button("Quit",GUILayout.Width(buttonWidth))){
					isQuiting = true;							
					Application.Quit();	
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
*/

			if(!TP_Motor.oculusRift){
				GUILayout.BeginArea (new Rect((screenW - (buttonWidth/2)), screenH,buttonWidth,260));
				GUILayout.BeginVertical();					
				if(GUILayout.Button("Start",GUILayout.Width(buttonWidth))){							
					tutorialCamera.isActivated = true;						
					if(soundNotice!=null){
						Destroy(soundNotice.gameObject);
					}
					Screen.lockCursor = true;
					if(disableIntroMusicAtStart) GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().reduceVolumeTemp();						
					//notesNotice.enabled=true;						
				}
				if(GUILayout.Button("Level 2",GUILayout.Width(buttonWidth))){						
					
					LevelState.created=false;
					Destroy(LevelState.getInstance().gameObject);
					LevelState.mainMenuOrder=false;
					loadingLevel=true;												
				}
				if(GUILayout.Button("Controls",GUILayout.Width(buttonWidth))){
					isControls = true;				
					controlsFromMenu=true;
				}
				/*
					if(GUILayout.Button("Credits",GUILayout.Width(buttonWidth))){
						isCredits = true;
					}	*/				
				//Exiting the application doesn't work in web build, so disable the button
				if(!Application.isWebPlayer){
					if(GUILayout.Button("Quit",GUILayout.Width(buttonWidth))){
						isQuiting = true;							
						Application.Quit();	
					}
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}
		if(isCredits){
			Time.timeScale = 0.01f;
			isMainMenu = false;
			
			//Put button images up first, then the buttons on top
			GUI.DrawTexture(new Rect((screenW - creditImage.width/4), screenH-creditImage.height/4-50, creditImage.width/2, creditImage.height/2),creditImage);
			
			//Return to the main menu
			if(GUI.Button(new Rect((screenW - buttonWidth/2), screenH+creditImage.height/5, buttonWidth, 100),"Back")){
				isCredits = false;
				isMainMenu = true;
			}
		}
		if(isControls){
			Time.timeScale = 0.01f;
			
			isPaused=false;
			isMainMenu=false;						
			
			//Put button images up first, then the buttons on top
			GUI.DrawTexture(new Rect((screenW - controlsImage.width/3), screenH-controlsImage.height/4-50, controlsImage.width/1.5f, controlsImage.height/2),controlsImage);
			
			//Return to the main menu
			if(GUI.Button(new Rect((screenW - buttonWidth/2), screenH+creditImage.height/6, buttonWidth, 100),"Back")){
				isControls=false;
				isPaused = controlsFromPause;
				isMainMenu = controlsFromMenu;
				controlsFromPause=false;
				controlsFromMenu=false;
				
			}
		}
	}
	
}
