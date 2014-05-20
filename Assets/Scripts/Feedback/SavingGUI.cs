using UnityEngine;
using System.Collections;

public class SavingGUI : MonoBehaviour {
	private GUIText savingText;
	private bool showText = false;
	private float startTime = 0f;
	public float timeToShow = 3f;
	// Use this for initialization
	void Start () {
		savingText = gameObject.GetComponent<GUIText>();
		
		savingText.fontSize = (int)( savingText.fontSize / 1024f * Screen.width );
		//savingText.pixelOffset = new Vector2( (savingText.pixelOffset.x / 1024) * Screen.width, (savingText.pixelOffset.y / 768) * Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
		if(showText)
		{
			if(Time.time - startTime < timeToShow)
			{
				savingText.enabled = true;	
			}
			else
			{
				savingText.enabled = false;
				showText = false;
			}
		}
	}
	
	public void ShowSavingText()
	{
		showText = true;
		startTime = Time.time;
	}
}
