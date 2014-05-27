using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowText : MonoBehaviour {
	
	internal bool showing = false;
	internal Queue<string> messageQueue;
	private Queue<float> xPosQueue;
	private Queue<float> yPosQueue;
	private Queue<bool> isDialogueQueue;
	
	public float timeToAppear=1.0f;
	public float timeToStay=3.0f;
	private float time=0f;
	public HelpKeys keyToAccept = HelpKeys.Interact;
	public GUIText text;	
	public GUIText[] outlines;	
	private bool disappearing=false;
	internal Color alphaColor;	
	internal Color betaColor;	
	private bool waitingText=false;	
	public Font dialogueFont;
	public Font systemFont;	
	public GameObject button;
	public Texture2D dialogueTex;
	public Texture2D systemTex;

	public int maxCharsPerLine=22;
	
	private string message="";
	private bool changedText=false;
	public bool dialogueWindows=true;


	public void ShowMessage(string text, float xPosition=0.4f, float yPosition=0.4f, bool systemText=false){
		if (TP_Motor.oculusRift) {
			xPosition =0.52f;
			yPosition = 0.48f;
			this.text.fontSize=40;
			foreach(GUIText outline in outlines){
				outline.fontSize=40;
			}
		}

		text = text.Replace("\n"," ");
		text = text.Replace("  "," ");

		char[] textArray = text.ToCharArray ();

		int pos = 0;
		while (pos+maxCharsPerLine < text.Length) {
			pos += maxCharsPerLine;
			char c = textArray[pos];

			while(c!=' ' && pos>0){
				pos--;
				c=textArray[pos];
			}

			if(c==' '){

				textArray[pos]='\n';
			}

		}
		string finalText = new string(textArray);
		messageQueue.Enqueue(finalText);
		xPosQueue.Enqueue(xPosition);
		yPosQueue.Enqueue(yPosition);
		isDialogueQueue.Enqueue(!systemText);
	}		
	
	void changeToDialogueFont(){		
		text.font=dialogueFont;
		foreach(GUIText outline in outlines){
			outline.font=dialogueFont;		
		}
		
		text.material = dialogueFont.material;	
		button.guiTexture.texture = dialogueTex;
		foreach (GUIText outline in outlines) {
			outline.material = dialogueFont.material;
		}
	}
	
	void changeToSystemFont(){
		text.font=systemFont;
		foreach (GUIText outline in outlines) {
			outline.font = systemFont;		
		}
		
		text.material = systemFont.material;
		button.guiTexture.texture = systemTex;
		foreach (GUIText outline in outlines) {
			outline.material = systemFont.material;
		}
	}
	
	// Use this for initialization
	void Start () {
		messageQueue = new Queue<string>();		
		xPosQueue = new Queue<float>();	
		yPosQueue = new Queue<float>();	
		isDialogueQueue = new Queue<bool>();
		alphaColor = new Color(1.0f,1.0f,1.0f,0.0f);
		betaColor = new Color(0.0f,0.0f,0.0f,0.0f);
		
		text.fontSize = (int)( text.fontSize / 600f * Screen.width );
		text.pixelOffset = new Vector2( (text.pixelOffset.x / 1024) * Screen.width, (text.pixelOffset.y / 768) * Screen.height);

		foreach (GUIText outline in outlines) {
			outline.fontSize = text.fontSize;
		}
		outlines[0].pixelOffset = text.pixelOffset + new Vector2(2f,0f);
		outlines[1].pixelOffset = text.pixelOffset + new Vector2(-2f,0f);
		outlines[2].pixelOffset = text.pixelOffset + new Vector2(0f,2f);
		outlines[3].pixelOffset = text.pixelOffset + new Vector2(0f,-2f);
		
	}
	
	void OnGUI(){		
		text.material.color = alphaColor;

		//button.guiTexture.color = alphaColor;
		foreach (GUIText outline in outlines) {
			outline.material.color = betaColor;
		}
		
		if(!changedText && showing){ 
			text.text=message;	
			foreach (GUIText outline in outlines) {
				outline.text=message;
			}
			changedText=true;
		}
		
	}
	
	// Update is called once per frame
	void Update () {		
		if(showing){
			text.material.color = alphaColor;
			//button.guiTexture.color = alphaColor;
			foreach (GUIText outline in outlines) {
				outline.material.color = betaColor;
			}
		}
		if(!showing && messageQueue.Count>0){
			message = messageQueue.Dequeue();
			float xPos = xPosQueue.Dequeue();
			float yPos = yPosQueue.Dequeue();

			if(message!="") Invoke("ActivateButton",0f);

			bool dialogue = isDialogueQueue.Dequeue();
			if(dialogue){
				changeToDialogueFont();
			} else {
				changeToSystemFont();
			}			
			
			showing=true;
							
			text.transform.position = new Vector3(xPos,yPos,1f);
			foreach (GUIText outline in outlines) {
				outline.transform.position = new Vector3(xPos,yPos,0f);
			}
																
			//text.text=message;	
			//outline.text=message;	
			
			changedText=false;
			
			time=0f;
		}
		if(!disappearing){
			if(!waitingText){
				if(showing){
					if(time<timeToAppear){
						time+=Time.deltaTime;
						alphaColor.a +=Time.deltaTime / timeToAppear;						
					} else {
						alphaColor.a = 1.0f;
						time=0f;
						waitingText=true;
					}					
				}
			} else {
				if(keyToAccept==HelpKeys.None){
					//time based
					if(time<timeToStay){
						time+=Time.deltaTime;
					}
					
					if(time>timeToStay){
						disappearing=true;						
						time=0f;
					}
				} else if(Input.GetButtonDown(HelpManager.getButtonName(keyToAccept))){								
					disappearing=true;
					if(messageQueue.Count==0 || messageQueue.Peek()=="") Invoke("DeactivateButton",0.1f);
					time=0f;
				}
			}
		} else {	
			if(time<=timeToAppear){
				time+=Time.deltaTime;			
				alphaColor.a-=Time.deltaTime / timeToAppear;
			} else {
				alphaColor.a=0f;				
				showing=false;
				waitingText=false;
				text.text="";
				foreach (GUIText outline in outlines) {
					outline.text="";
				}
				time=0f;
				disappearing=false;
			}
		}
		betaColor.a = alphaColor.a;
	}

	public void ActivateButton(){
		if(dialogueWindows) button.SetActive (true);
	}

	public void DeactivateButton(){
		if(dialogueWindows) button.SetActive (false);
	}
	
	public void HideText(){
		if (showing) {
			disappearing = true;
			Invoke("DeactivateButton",0.1f);
		}
	}
	
}
