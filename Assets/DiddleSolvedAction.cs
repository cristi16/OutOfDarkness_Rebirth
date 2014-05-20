using UnityEngine;
using System.Collections;

public class DiddleSolvedAction : Action {
	
	public GameObject background;
	public Texture2D solved;
	private bool deactivate=false;
	private ShowText showText;
	public Collider blockCollider;
	public GUIText text;
	
	private bool toBeContinued=false;
	private bool wait=false;
	private float time=0f;
	public PuzzleController puzzle;
	
	// Use this for initialization
	void Start () {
		showText = GameObject.FindGameObjectWithTag("HelpManager").GetComponent<ShowText>();
	}
	
	// Update is called once per frame
	void Update () {
		if(deactivate && showText.messageQueue.Count==0 && !showText.showing && GetComponent<TextTrigger>().shown){		
			deactivate=false;			
			DeactivatePuzzle();
		}
		
		if(toBeContinued){
			float a = text.material.color.a+Time.deltaTime;
			if(a>=1f){
				a=1f;
				toBeContinued=false;
				wait=true;
			}
			text.material.color = new Color(1f,1f,1f,a);
		}
		
		if(wait){
			time+=Time.deltaTime;
			
			if(time>=3f){
				LevelState.created=false;
				Destroy(LevelState.getInstance().gameObject);
				LevelState.mainMenuOrder=true;
				Application.LoadLevel(0);
			}
		}
		
	}
	
	void DeactivatePuzzle(){
		puzzle.DeactivatePuzzle();
		text.material.color = new Color(1f,1f,1f,0f);
		text.enabled=true;
		toBeContinued=true;
		GameObject.FindGameObjectWithTag("SceneFader").GetComponent<SceneFadeInOut>().fadeToBlack=true;
		
	}
	
	public override void execute(){								
		
		blockCollider.enabled=true;
		
		LevelState.getInstance().diddleSolved=true;
		
		background.GetComponent<MeshRenderer>().material.mainTexture=solved;
		
		puzzle.canExit=false;
		
		GetComponent<TextTrigger>().ShowMessages();		
		
		deactivate=true;
	}
}
