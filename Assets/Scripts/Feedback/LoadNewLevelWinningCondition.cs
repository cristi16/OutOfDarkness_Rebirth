using UnityEngine;
using System.Collections;

public class LoadNewLevelWinningCondition : MonoBehaviour {
	
	private CheckpointsManager_Script end_level;
	private SceneFadeInOut fader;
	private bool showTransitionTexture = false;
	private bool fadeToTexture = false;
	private GUITexture transitionTexture;
	public float timeToShow = 6f;
	private float timeCounter = 0f;
	
	// Use this for initialization
	void Start () {
		
		transitionTexture = GameObject.Find("TransitionTexture").GetComponent<GUITexture>();		
		
		GameObject game_manager = GameObject.Find("Game Manager");
		fader = GameObject.FindGameObjectWithTag("SceneFader").GetComponent<SceneFadeInOut>();
		
		if(game_manager == null){
			Debug.LogError("Error inizialization of trigger AI script "+this.name);
			return;
		}
		
		end_level = game_manager.GetComponent<CheckpointsManager_Script>();
	}
	
		void OnTriggerStay(Collider other) {
        if (other.CompareTag("Kid"))
		{	
			if(!showTransitionTexture)
				fader.fadeToBlack = true;
			
			showTransitionTexture = true;
		}
    }
	
	private void Update()
	{
		if(showTransitionTexture)
		{
			if(fader.fadeToBlack && fader.GetComponent<GUITexture>().color.a > 0.95f)
			{
				fadeToTexture = true;
				fader.fadeToBlack = false;
			}
			else if(fadeToTexture)
			{
				transitionTexture.enabled = true;
				fader.fadeToClear = true;
				fadeToTexture = false;		
			}
			else if(timeCounter < timeToShow)
			{
				timeCounter += Time.deltaTime;
				if(fader.fadeToClear == true && fader.GetComponent<GUITexture>().color.a <= 0.05f) 
					fader.fadeToClear = false;
			}
			else
			{
				fader.fadeToBlack = true;
				showTransitionTexture = false;
				Invoke("LoadMainLevel", 1f);
			}
				
		}
	}
	
	private void LoadMainLevel()
	{
		LevelState.getInstance().ClearLevelState();
		LevelState.getInstance().usingMainMenu = false;
		end_level.LoadTestLevel();
	}
}
