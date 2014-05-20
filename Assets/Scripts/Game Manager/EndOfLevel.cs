using UnityEngine;
using System.Collections;

public class EndOfLevel : MonoBehaviour {
	
	public float fadeSpeed = 2f;
	private SwitchMechanic_Script switchMechanic;
	private bool fadeToWhite = false;
	public GameObject whiteTexture;
	
    void Awake ()
	{
        // Set the texture so that it is the the size of the screen and covers it.
        whiteTexture.guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
    }
	
	// Use this for initialization
	void Start () {
		switchMechanic = GameObject.FindGameObjectWithTag("GameController").GetComponent<SwitchMechanic_Script>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(fadeToWhite)
		{
			// Make sure the texture is enabled.
	        whiteTexture.guiTexture.enabled = true;
	        
	        whiteTexture.guiTexture.color = Color.Lerp(whiteTexture.guiTexture.color, Color.white, fadeSpeed * Time.deltaTime);
	        
	        // If the screen is almost white...
	        if(whiteTexture.guiTexture.color.a >= 0.95f)
			{
	            // ... go to main menu		
	            LevelState.getInstance().usingMainMenu = true;
				//LevelState.getInstance().swapActivatedFromStart = false;
				//LevelState.getInstance().ghostFollowsKidFromStart = false;
				LevelState.getInstance().ClearLevelState();
				Application.LoadLevel(0);
			}
		}
	}
	
	private void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.tag == "Kid")
		{
			switchMechanic.DisableControlEndOfLevel();	
		}
	}
	
	private void OnTriggerExit(Collider hit)
	{
		if(hit.gameObject.tag == "Kid")
		{
			fadeToWhite = true;
		}	
	}
}
