using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookForGhost : MonoBehaviour {
	
	public float transform_Y_offset = 1f;
	//public int indexOfIgnoredLayer = 8;
	public float safeAreaRadius = 10f;
	private Transform ghostTransform;
	private AudioSource audioSource;
	private int ignoredLayerMask;
	private RaycastHit hitInfo;
	private bool canSeeGhost = true;
	
	private float minimumTimeToReachTarget = 0.2f;
	public float maximumRotateSpeed = 200f;
	//Sound variables
	public int chasing_nuns_number = 3;
	private bool incoming_nun = false;
	GameObject[] nuns;
	
	public float minimum_sound = 0.0f;
    public float maximum_sound = 1.0f;
    public float timeToFadeIn = 10.0f;
    public float timeToFadeOut = 3.0f;
	
    private bool _isFadingIn = false;
    private bool _isFadingOut = false;
	
	public float time_every_chase = 10f;
	private float chase_timer;
	
	public AudioClip scream;
	public AudioClip sniffle;
	private bool sniff= true;
	
	void Start() 
	{
		chase_timer = Time.time;
		
		ghostTransform = GameObject.Find("Ghost").transform;
		GameObject temp = GameObject.Find("Game Manager");
		
		if(ghostTransform == null || temp == null){
			Debug.LogError("Inizialization error in LookForGhost");
			return;
		}
		
		nuns = GameObject.FindGameObjectsWithTag("Nun");
		
		audioSource = ghostTransform.gameObject.GetComponent<AudioSource>();
		
		//ignoredLayerMask = 1 << indexOfIgnoredLayer;
		// if we have certain objects on the Ignore Raycast Layer  and we 
		// are using a layer mask we have to add the Ignore Raycast index to our layer mask,
		// Otherwise the layer won't be taken into account in the current situation
		ignoredLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
		// We have to specify which layer will be taken into account, which is why negate our mask
		ignoredLayerMask = ~ignoredLayerMask;
	}
	
	void Update() 
	{
		if(!SwitchMechanic_Script.getKidControl()) //Don't check if the kid has the control
		{
//			FadeOut(); //fade out if we switch back to the kid
//			return;
			Vector3 newRotation = Quaternion.LookRotation(ghostTransform.position - transform.position).eulerAngles;
	        Vector3 angles = transform.rotation.eulerAngles;
			float _velocity = 0f;
	        transform.rotation = Quaternion.Euler(angles.x, Mathf.SmoothDampAngle(angles.y, newRotation.y, 
			ref _velocity, minimumTimeToReachTarget, maximumRotateSpeed), angles.z);
		}
		
		
		
		if((transform.position - ghostTransform.position).sqrMagnitude > safeAreaRadius * safeAreaRadius)
		{
			CheckLineOfSight();
		}
		else
		{
			canSeeGhost = true;
			CancelInvoke("FindGhost");	
		}
		
		if(canSeeGhost == false)
		{							
			FadeIn();
			
			Debug.DrawLine(transform.position, hitInfo.point);
		}
		else
		{							
			FadeOut();
		}
		
		if(incoming_nun)
			callNuns(); //Check if they have finished to chase the kid in order to clear the list and reset the variable
	}
	
	void CheckLineOfSight()
	{
		if(IsInvoking("FindGhost") == false)
			InvokeRepeating("FindGhost", 0, 1);
	}
	
	void FindGhost()
	{	
		if(Physics.Linecast(transform.position + new Vector3(0, transform_Y_offset, 0), 
			ghostTransform.position + new Vector3(0, transform_Y_offset, 0), out hitInfo, ignoredLayerMask))
		{
			if(hitInfo.transform.gameObject.tag == ghostTransform.gameObject.tag)
				canSeeGhost = true;
			else
				canSeeGhost = false;
		}
		else
		{
			canSeeGhost = false;
		}
	}
 
    public void FadeIn()
    {
		
		if (audioSource.volume>=0.5f && audioSource.volume<0.8f && sniff==true){
			//Debug.Log("sniffle");
			audioSource.PlayOneShot(sniffle,1);
			sniff=false;
		}
		if (audioSource.volume>=0.8f && sniff==false){
			//Debug.Log("sniffle");
			audioSource.PlayOneShot(sniffle,1);
			sniff=true;
		}
		
        this._isFadingOut = false; 
		if(_isFadingIn == false && audioSource.volume < maximum_sound)
		{
        	StartCoroutine(DoFadeIn());
		}else if(audioSource.volume >= maximum_sound && !incoming_nun){ //Time to calling the nunzZz
			incoming_nun = true;
		}
    }
 
    public void FadeOut()
    {
        this._isFadingIn = false; 
		if(_isFadingOut == false && audioSource.volume > minimum_sound)
		{
        	StartCoroutine(DoFadeOut());
		}
    }
 
    private IEnumerator DoFadeIn()
    {
        this._isFadingIn = true;
 
        float startTime = Time.time;
        float elapsedTime = 0;
		
		if(audioSource.volume == minimum_sound) // fade in process starts from zero
		{
			audioSource.Play();
			//Debug.Log("Start");	
		}
		
		float ratio = 1 - audioSource.volume / maximum_sound;
		minimum_sound = audioSource.volume;		
		
        do
        {
            elapsedTime = Time.time - startTime;
            audioSource.volume = Mathf.Lerp(this.minimum_sound, this.maximum_sound, elapsedTime / (timeToFadeIn * ratio));
            yield return null;
			
        } while( this._isFadingIn && elapsedTime < this.timeToFadeIn * ratio);      
		
		minimum_sound = 0;
        this._isFadingIn = false;
    }
 
    private IEnumerator DoFadeOut()
    {
        this._isFadingOut = true;
 
        float startTime = Time.time;
        float elapsedTime = 0;
		
		float ratio = audioSource.volume / maximum_sound;
		maximum_sound = audioSource.volume;
		
        do
        {	
            elapsedTime = Time.time - startTime;
            audioSource.volume = Mathf.Lerp(this.maximum_sound, this.minimum_sound, elapsedTime / (timeToFadeOut * ratio));
            yield return null;
			
        } while( this._isFadingOut && elapsedTime < this.timeToFadeOut * ratio);
 		
		// POSSIBLE BUG ---- Audio source is stopped and the volume starts fading in
		
		if(_isFadingIn == false) // fade out process complete
		{
			audioSource.Stop();
			//Debug.Log("Stop");	
		}

		this._isFadingOut = false;
		maximum_sound = 1;
    }
	
	void callNuns(){
		if(chase_timer < Time.time){
			audioSource.PlayOneShot(scream,1);
			float[] distances = new float[chasing_nuns_number];
			int[] indeces = new int[chasing_nuns_number];
			
			for(int i = 0; i < chasing_nuns_number; i++)
				distances[i] = 999999f;
	
			if(nuns.Length > 0){
				for (int i=0; i < nuns.Length; i++){
					float temp = Vector3.Distance(nuns[i].transform.position, transform.position);
							
					for(int j = 0; j < chasing_nuns_number; j++){
						if ( temp <= distances[j]){
							distances[j] = temp;
							indeces[j] = i;
							break;
						}
					}
				}
				
				
				for(int i = 0; i < chasing_nuns_number; i++){
					AI temp = nuns[indeces[i]].GetComponent<AI>();
					temp.activateChasingInvestigate(transform.gameObject,0.5f); //All the nearest nuns have to investigate
				}	
			}
			
			chase_timer = Time.time + time_every_chase;
			incoming_nun = false;
		}			
	}
}
