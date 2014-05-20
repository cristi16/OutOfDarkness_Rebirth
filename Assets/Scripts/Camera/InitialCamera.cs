using UnityEngine;
using System.Collections;

public class InitialCamera : MonoBehaviour {
	
	public Transform target;
	public float acceleration = 1.001f;
	public float deceleration = 0.999f;
	public float rotationSpeed = 2f;
	
	private float travelSpeed  = 60f;
	private float minDistance;
	private float totalDistance;
	private Vector3 direction;
	private bool isTraveling = true;
	private float currentDistance;
	private bool positionSet = false;
	private CheckpointsManager_Script gameManager;
	private Light cameraLight;
	private float timeForLastFix = 1f;
	private float adjustmentSpeed = 2f;
	private float time = 0f;
	private float timeBeforeStart = 2f;
	private float startTime = 0f;
	
	private Vector3 targetOffset;
	
	// Use this for initialization
	void Start () {
		targetOffset = new Vector3(0, transform.position.y - target.position.y, 0);
		
		minDistance = Camera.mainCamera.GetComponent<TP_Camera>().distanceMin;
		totalDistance = Vector3.Distance(transform.position, target.position);
		direction = Vector3.Normalize( (target.position + targetOffset) - transform.position);
		cameraLight = gameObject.GetComponentInChildren<Light>();
		GameObject temp = GameObject.FindGameObjectWithTag("GameController");
		gameManager = temp.GetComponent<CheckpointsManager_Script>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(gameManager.playIntroCamera == false) Destroy(gameObject);
		
		if(startTime < timeBeforeStart)
		{
			startTime += Time.deltaTime;
			return;
		}
		
		if(isTraveling)
		{
			currentDistance = Vector3.Distance(transform.position, (target.position + targetOffset) );
			
			if( currentDistance > minDistance)
			{
				if(currentDistance > totalDistance / 2 )
					travelSpeed *= deceleration;
				else
					travelSpeed *= acceleration;
				Vector3 step = direction * travelSpeed * Time.deltaTime;
				transform.position += new Vector3(step.x, 0, step.z);	
				
				if( currentDistance < 2 * minDistance)
				{
					cameraLight.intensity = Mathf.Lerp(cameraLight.intensity, 0, 2f * Time.deltaTime);
					cameraLight.range = Mathf.Lerp(cameraLight.range, 0, 2f * Time.deltaTime);
				}
			}
			if(currentDistance < minDistance)
				isTraveling = false;
		}
		else
		{
			float dot = Vector3.Dot(-(target.forward +targetOffset), Vector3.Normalize(transform.position - (target.position + targetOffset) ));
			if( Mathf.Abs( dot + 1 ) < 1.95f)
			{
				transform.RotateAround((target.position), target.right, rotationSpeed);
				transform.rotation = Quaternion.Euler(target.transform.eulerAngles);
				
			}
			else if(!positionSet)
			{
				transform.position = Vector3.Lerp(transform.position, Camera.mainCamera.transform.position, adjustmentSpeed * Time.deltaTime);
				if(Vector3.Distance(transform.position, Camera.mainCamera.transform.position) < 0.2f)
					positionSet = true;
			}
			else
			{
				transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.eulerAngles.x, Camera.mainCamera.transform.localEulerAngles.x, adjustmentSpeed * Time.deltaTime),
					Mathf.Lerp(transform.eulerAngles.y, Camera.mainCamera.transform.localEulerAngles.y, adjustmentSpeed * Time.deltaTime),
					Mathf.Lerp(transform.eulerAngles.z, Camera.mainCamera.transform.localEulerAngles.z, adjustmentSpeed * Time.deltaTime) );
				time += Time.deltaTime;
				if(time > timeForLastFix)
				{
					gameManager.playIntroCamera = false;
					Destroy(this.gameObject);
				}
			}
		}
		
		
		

	}
}
