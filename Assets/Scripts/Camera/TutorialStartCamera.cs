using UnityEngine;
using System.Collections;

// Animates the position in an arc between sunrise and sunset.
public class TutorialStartCamera : MonoBehaviour {
	
	public Transform target;
	// Time to move from sunrise to sunset position, in seconds.
	public float journeyTime = 6f;
	public bool isActivated = false;
	internal float guiAlpha = 1f;
	
	private bool firstTime = true;
	private CheckpointsManager_Script gameManager;
	private MenuManager menuManager;
	private Light cameraLight;
	
	private Vector3 sunrise;
    private Vector3 sunset;
   	 // The time at which the animation started.
    private float slerpStartTime = 0f;
	private float startTime = 0f;
	private float timeBeforeStart = 0.2f;
	
	private float lightIntensity;
	private float lightRange;	
	public Camera gameCamera;
	
	// Use this for initialization
	void Start () {
		
		cameraLight = gameObject.GetComponentInChildren<Light>();
		lightIntensity = cameraLight.intensity;
		lightRange = cameraLight.range;
		
		GameObject temp = GameObject.FindGameObjectWithTag("GameController");
		gameManager = temp.GetComponent<CheckpointsManager_Script>();
		menuManager = temp.GetComponent<MenuManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(menuManager.isMainMenu == false && menuManager.isCredits == false && menuManager.isControls == false) 
			Destroy(gameObject);
		
		if(!isActivated) return;
		
		if(startTime < timeBeforeStart)
		{
			startTime += Time.deltaTime;
			return;
		}
		
		if(firstTime)
		{
			sunrise = gameObject.transform.position;
			sunset = Camera.mainCamera.transform.position;
			slerpStartTime = Time.time;
			firstTime = false;
		}
		else
		{
			// The center of the arc
			Vector3 center = (sunrise + sunset) * 0.5f;
			// move the center a bit to the side to make the arc horizontal
	        center -= new Vector3(0, 0, 1);
			// Interpolate over the arc relative to center
	        Vector3 riseRelCenter = sunrise - center;
	        Vector3 setRelCenter = sunset - center;
			// The fraction of the animation that has happened so far is
        	// equal to the elapsed time divided by the desired time for
        	// the total journey.
	 		float fracComplete = (Time.time - slerpStartTime) / journeyTime;
	        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
	        transform.position += center;
			
			if(gameCamera==null){
				// Interpolate the rotation so that we always look at the target
				Quaternion lookAtRotation = Quaternion.LookRotation(target.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, fracComplete);	
			} else {
				// Interpolate the rotation so that the camera looks the same way as the game camera				
				transform.rotation = Quaternion.Slerp(transform.rotation, gameCamera.transform.rotation, fracComplete);	
				
				//Interpolate field of view
				camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, gameCamera.fieldOfView, fracComplete/2);
			}
			// fade out light
			cameraLight.intensity = Mathf.Lerp(lightIntensity, 0f, fracComplete);
			cameraLight.range = Mathf.Lerp(lightRange, 0f, fracComplete);
			// fade out gui
			guiAlpha =  Mathf.Clamp(1f - (Time.time - slerpStartTime) / (journeyTime * 0.3f), 0f, 1f);
			
			if(fracComplete >=1)
			{
				gameManager.GetComponent<MenuManager>().isMainMenu = false;
				GameObject.FindGameObjectWithTag("Kid").GetComponent<TP_Controller>().returnControl(true);				
				Destroy(this.gameObject);
			}
		}
	}
}
