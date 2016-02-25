using UnityEngine;
using System.Collections;

public enum CameraStates
{
	Default = 0,
	LookAtLerp = 1,
	ReturnToDefault = 2
}

public class PuzzleCamera : MonoBehaviour {
	
	private Transform cameraTransform;
	private Transform target;
	private Quaternion lookAtRotation;
	private Vector3 defaultPosition;
	private CameraStates currentState;
	private TP_Controller inputController;
	
	private Vector3 sunrise;
    private Vector3 sunset;
    private float slerpStartTime = 0f; // The time at which the animation started.
	private float cameraJourneyTime;
	
	// Use this for initialization
	void Start () {
		currentState = CameraStates.Default;
		cameraTransform = Camera.main.transform;
		inputController = Camera.main.transform.root.GetComponentInChildren<TP_Controller>();
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentState)
		{
			case CameraStates.LookAtLerp:
				//MOVE TO TARGET
				SmoothMove(true);
				// ROTATE FIRST PERSON CONTROLLER
				transform.rotation = Quaternion.Slerp(transform.rotation,
				lookAtRotation, Time.deltaTime * 3f);
				// ALIGN CAMERA
				Vector3 localAngles = cameraTransform.localEulerAngles;
				cameraTransform.localEulerAngles = new Vector3(Mathf.LerpAngle(localAngles.x, 0, 3f * Time.deltaTime),
				localAngles.y, localAngles.z);
				break;
			case CameraStates.ReturnToDefault:
				// MOVE TO TARGET
				float fracComplete = SmoothMove(false);
			
				if(fracComplete >=1)
				{		
					currentState = CameraStates.Default;
					inputController.returnControl();
				}
				break;
			case CameraStates.Default:
				break;
		}
	}
	
	public void ActivateLookAt(Transform target, float cameraStopDistance, float journeyTime)
	{
		this.cameraJourneyTime = journeyTime;
		inputController.removeControl();
		
		this.target = target;
		lookAtRotation = Quaternion.LookRotation(-target.forward);
		defaultPosition = transform.position;
			
		sunrise = transform.position;
		sunset = target.position + target.forward * cameraStopDistance;
		slerpStartTime = Time.time;
			
		currentState = CameraStates.LookAtLerp;
	}
	
	public void DeactivateLookAt()
	{
		sunrise = transform.position;
		sunset = target.position + new Vector3(target.forward.x * 2f, defaultPosition.y, target.forward.z * 2f);
		slerpStartTime = Time.time;
		
		currentState = CameraStates.ReturnToDefault;	
	}
	
	private float SmoothMove(bool slerp)
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
 		float fracComplete = (Time.time - slerpStartTime) / cameraJourneyTime;
        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
		
		return fracComplete;
	}
}
