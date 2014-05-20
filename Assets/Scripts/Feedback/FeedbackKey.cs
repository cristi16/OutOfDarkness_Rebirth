using UnityEngine;
using System.Collections;

public class FeedbackKey : MonoBehaviour {
	
	private HelpManager control;
	internal bool active=false;
	public float timeToAppear=1.0f;
	public float timeToStay=5.0f;
	private GUITexture texture;
	
	private float upperLimit = 0.5f;
	
	public float fadeInRatio=0.5f;
	private bool appearing=true;
	private bool disappearing=true;
	private float time=0f;
	internal bool removeIt=false;
	
	// Use this for initialization
	void Start () {
		texture = GetComponent<GUITexture>();
		texture.color = new Color(texture.color.r,texture.color.g,texture.color.b,0);
		
		texture.pixelInset = ResizeGUI(texture.pixelInset);
	}
	
	// Update is called once per frame
	void Update () {
		if(active){
			if(appearing){
				if(texture.enabled) texture.color = new Color(texture.color.r,texture.color.g,texture.color.b,texture.color.a+Time.deltaTime*fadeInRatio);
				
				if(texture.color.a>=upperLimit){
					appearing=false;
					texture.color = new Color(texture.color.r,texture.color.g,texture.color.b,upperLimit);
				}
			} else if (time<=timeToStay && !removeIt){
				if(texture.enabled) time+=Time.deltaTime;				
			} else if(disappearing){
				if(texture.enabled) texture.color = new Color(texture.color.r,texture.color.g,texture.color.b,texture.color.a-Time.deltaTime*fadeInRatio);
				
				if(texture.color.a<=0){
					texture.color = new Color(texture.color.r,texture.color.g,texture.color.b,0f);
					disappearing=false;					
				}				
			} else {
				active=false;
				appearing=true;
				disappearing=true;
				time=0f;
			}
		}
	}
	
	private Rect ResizeGUI(Rect _rect)
	{
	    float FilScreenWidth = _rect.width / 1024;
	    float rectWidth = FilScreenWidth * Screen.width;
	    float FilScreenHeight = _rect.height / 768;
	    float rectHeight = rectWidth * _rect.height / (float) _rect.width;
	    float rectX = (_rect.x / 1024) * Screen.width;
	    float rectY = (_rect.y / 768) * Screen.height;
	 
	    return new Rect(rectX,rectY,rectWidth,rectHeight);
	}
}
