using UnityEngine;

public static class Helper
{
	public struct ClipPlanePoints
	{
		public Vector3 upperLeft;	
		public Vector3 upperRight;
		public Vector3 lowerLeft;
		public Vector3 lowerRight;
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		do 
		{
			if(angle < -360)
				angle +=360;
			if(angle > 360)
				angle -=360;
		} while (angle < -360 || angle > 360);
		
		return Mathf.Clamp(angle, min, max);
	}
	
	public static ClipPlanePoints ClipPlaneAtNear(Vector3 position)
	{
		ClipPlanePoints clipPlanePoints = new ClipPlanePoints();
		
		if (Camera.main == null)
			return clipPlanePoints;
		
		Transform transform = Camera.main.transform;
		float halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
		float aspect = Camera.main.aspect;
		float distance = Camera.main.nearClipPlane;
		
		float height = distance * Mathf.Tan(halfFOV);
		float width = height * aspect;
		
		clipPlanePoints.lowerRight = position + transform.right * width;
		clipPlanePoints.lowerRight -= transform.up * height;
		clipPlanePoints.lowerRight += transform.forward * distance;
		
		clipPlanePoints.lowerLeft = position - transform.right * width;
		clipPlanePoints.lowerLeft -= transform.up * height;
		clipPlanePoints.lowerLeft += transform.forward * distance;
		
		clipPlanePoints.upperRight = position + transform.right * width;
		clipPlanePoints.upperRight += transform.up * height;
		clipPlanePoints.upperRight += transform.forward * distance;
		
		clipPlanePoints.upperLeft = position - transform.right * width;
		clipPlanePoints.upperLeft += transform.up * height;
		clipPlanePoints.upperLeft += transform.forward * distance;
		
		return clipPlanePoints;
	}
}


