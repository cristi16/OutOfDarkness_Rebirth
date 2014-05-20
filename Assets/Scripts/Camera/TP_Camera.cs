using UnityEngine;
using System.Collections;

// NEEDS LOTS OF TWEAKING BASED ON THE ACTUAL CHARACTER AND MODEL ( there might be problems when the camera gets occluded)
public class TP_Camera : MonoBehaviour 
{
	public static TP_Camera instance;
	
	public Transform targetLookAt; // should have the same tag as the parent gameobject
	public float distance = 5f;
	public float distanceMin = 3f;
	public float distanceMax = 10f;
	public float distanceSmooth = 0.05f;
	public float distanceResumeSmooth = 1f;
	public float xMouseSensitivity = 5f;
	public float yMouseSensitivity = 5f;
	public float mouseWheelSensitivity = 5f;
	public float xSmooth = 0.05f;
	public float ySmooth = 0.1f;
	public float yMinLimit = -40f;
	public float yMaxLimit = 80f;
	public float occlusionDistanceStep = 0.5f;
	public int maxOcclusionChecks = 10;
	
	private float _mouseX = 0f;
	private float mouseY = 0f;
	private float velX = 0f;
	private float velY = 0f;
	private float velZ = 0f;
	private float velocityDistance = 0f;
	private float startDistance = 0f;
	private Vector3 position = Vector3.zero;
	private float desiredDistance = 0f;
	private Vector3 desiredPosition = Vector3.zero;
	private float _distanceSmooth = 0f;
	private float preOccludedDistance =0f;
	private bool firstTimeUpdate = true;
	private int ignorePlayerMask;
	internal bool ignoreInput = false;
	
	public float constantHeight = 20f;
	public bool rightClickToMoveCamera=true;
	
	
	void Awake()
	{
		instance=this;
	}

	void Start() 
	{
		ignorePlayerMask = 1 << LayerMask.NameToLayer("Player");
		ignorePlayerMask += 1 << LayerMask.NameToLayer("Ignore Raycast");
		ignorePlayerMask += 1 << LayerMask.NameToLayer("GhostCollider");
		ignorePlayerMask = ~ignorePlayerMask;
		
		distance = Mathf.Clamp(distance, distanceMin, distanceMax);
		startDistance = distance;
		Reset();
	}
	
	void LateUpdate()
	{
		if(targetLookAt == null)
			return;
		
		if(!firstTimeUpdate && ignoreInput) return;
		
		HandlePlayerInput();
		
		int count = 0;
		
		do 
		{
			CalculateDesiredPosition();
			count++;
		} while (CheckIfOccluded(count));
		
		// Fade in / out the character when the camera gets closer / further from the character
//		Material mat = targetLookAt.parent.gameObject.renderer.material;
//		float clampedDistance = Mathf.Clamp(distance, 2, 4);
//		float alpha = Mathf.Lerp(0f, 1f, clampedDistance / 4);
//		mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, alpha));
		
		UpdatePosition();
	}
	
	void HandlePlayerInput()
	{
		float deadZone = 0.01f;
		
		//checking for right click for debug purposes. !!to be removed
		if(!rightClickToMoveCamera || Input.GetMouseButton(1))
		{
			// the RMB is down. Get mouse Axis input
			_mouseX += Input.GetAxis("Mouse X") * xMouseSensitivity;
			//mouseY -= Input.GetAxis("Mouse Y") * yMouseSensitivity;
			mouseY = constantHeight; // default height position
		}
		
		// This is where we will limit our mouseY rotation
		mouseY = Helper.ClampAngle(mouseY, yMinLimit, yMaxLimit);
		// Ignoring camera zooming in and out on scroll
//		if(Input.GetAxis("Mouse ScrollWheel") < - deadZone || Input.GetAxis("Mouse ScrollWheel") >  deadZone)
//		{
//			desiredDistance = Mathf.Clamp( distance - Input.GetAxis("Mouse ScrollWheel") * mouseWheelSensitivity,
//				distanceMin, distanceMax);	
//			preOccludedDistance = desiredDistance;
//			_distanceSmooth = distanceSmooth;
//		}
	}
	void CalculateDesiredPosition()
	{
		// Evaluate distance
		ResetDesiredDistance();
		distance = Mathf.SmoothDamp(distance, desiredDistance, ref velocityDistance, _distanceSmooth);
		
		// Calculate desired position
		desiredPosition = CalculatePosition( mouseY, _mouseX, distance);			
	}
	
	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		// configure the vector relative to which the camera's rotation and position is calculated
		Vector3 direction = new Vector3(-distance, 0, 0);
		Quaternion rotation = Quaternion.Euler(0, rotationY, -rotationX);
		
		return targetLookAt.position + rotation * direction;
	}
	
	bool CheckIfOccluded(int count)
	{
		bool isOccluded = false;
		
		float nearestDistance = CheckCameraPoints(targetLookAt.position, desiredPosition);
		
		if(nearestDistance != -1)
		{
			if(count < maxOcclusionChecks)
			{
				isOccluded = true;
				
				distance -= occlusionDistanceStep;
				
				if(distance < 1f) // if camera gets closer than 0.25 meters it will act weird. therefore, restrict its behaviour
				{
					distance = 1f;
				}
			}
			else
			{
				distance = nearestDistance - Camera.mainCamera.nearClipPlane;	// move to a safe point // possibly the same size as the raidus of the ch collider
				//Debug.Log("move to safe point: "+distance);			
			}
			
			desiredDistance = distance; // this makes sure no smoothing is applied
			_distanceSmooth = distanceResumeSmooth;
		}
		
		return isOccluded;
	}
	
	// from - the lookAtPoint
	// to - the camera position
	float CheckCameraPoints(Vector3 from, Vector3 to)
	{
		float nearestDistance = -1f;
		
		RaycastHit hitInfo;
		
		Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);
		
		// Draw lines in the Editor to make it easier to visualize
		Debug.DrawLine(from, to + this.transform.forward * -camera.nearClipPlane, Color.red); //line to a point behind the camera
		Debug.DrawLine(from, clipPlanePoints.upperLeft);
		Debug.DrawLine(from, clipPlanePoints.lowerLeft);
		Debug.DrawLine(from, clipPlanePoints.upperRight);
		Debug.DrawLine(from, clipPlanePoints.lowerRight);
		
		Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.upperRight);
		Debug.DrawLine(clipPlanePoints.upperRight, clipPlanePoints.lowerRight);
		Debug.DrawLine(clipPlanePoints.lowerRight, clipPlanePoints.lowerLeft);
		Debug.DrawLine(clipPlanePoints.lowerLeft, clipPlanePoints.upperLeft);
		
		if(Physics.Linecast(from, clipPlanePoints.upperLeft, out hitInfo, ignorePlayerMask) 
			&& hitInfo.collider.tag != targetLookAt.gameObject.tag && hitInfo.collider.isTrigger == false)
			nearestDistance = hitInfo.distance;

		if(Physics.Linecast(from, clipPlanePoints.lowerLeft, out hitInfo, ignorePlayerMask) 
			&& hitInfo.collider.tag != targetLookAt.gameObject.tag && hitInfo.collider.isTrigger == false)
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.upperRight, out hitInfo, ignorePlayerMask) 
			&& hitInfo.collider.tag != targetLookAt.gameObject.tag && hitInfo.collider.isTrigger == false)
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, clipPlanePoints.lowerRight, out hitInfo, ignorePlayerMask) 
			&& hitInfo.collider.tag != targetLookAt.gameObject.tag && hitInfo.collider.isTrigger == false)
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast(from, to + this.transform.forward * -camera.nearClipPlane, out hitInfo, ignorePlayerMask) 
			&& hitInfo.collider.tag != targetLookAt.gameObject.tag && hitInfo.collider.isTrigger == false)
			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		return nearestDistance;
	}
	
	void ResetDesiredDistance()
	{
		if(desiredDistance < preOccludedDistance)
		{
			Vector3 pos = CalculatePosition(mouseY, _mouseX, preOccludedDistance);
			
			float nearestDistance = CheckCameraPoints(targetLookAt.position, pos);
			
			if(nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;	
			}
		}
	}
	
	void UpdatePosition()
	{
		if(firstTimeUpdate)
		{
			position = desiredPosition;
			firstTimeUpdate = false;
		}
		else
		{
			if(!ignoreInput)
			{	
				float posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, xSmooth);
				float posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, ySmooth);
				float posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, xSmooth);
				
				position = new Vector3(posX, posY, posZ);
			}
		}
		
		transform.position = position;
		transform.LookAt(targetLookAt);
	}
	
	public void Reset()
	{
		_mouseX = 0;
		mouseY = 10;
		distance = startDistance;
		desiredDistance = distance;
		preOccludedDistance = distance;
	}
	
	public static void UseExistingOrCreateNewMainCamera()
	{
		GameObject tempCamera;
		GameObject tempTargetLookAt;
		
		TP_Camera myCamera;
		
		if(Camera.mainCamera != null)
		{
			tempCamera = Camera.mainCamera.gameObject;	
		}
		else
		{
			tempCamera = new GameObject("Main Camera");
			tempCamera.AddComponent("Camera");
			tempCamera.tag = "MainCamera";
		}
		
		
		tempCamera.AddComponent("TP_Camera");
		myCamera = tempCamera.GetComponent("TP_Camera") as TP_Camera;
		
//		tempTargetLookAt = GameObject.Find("targetLookAt") as GameObject;
//		
//		if(tempTargetLookAt == null)
//		{
//			tempTargetLookAt = new GameObject("targetLookAt");
//			tempTargetLookAt.transform.position = Vector3.zero;
//		}
//		
//		myCamera.targetLookAt=tempTargetLookAt.transform;
	}
}
