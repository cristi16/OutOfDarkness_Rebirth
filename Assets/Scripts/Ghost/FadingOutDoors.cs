using UnityEngine;
using System.Collections;

public class FadingOutDoors : MonoBehaviour {
	
	public int fadingRange = 6;
	
	private DoorInteraction door;
	private Renderer renderer;
	private GameObject ghost;
	private float fadeStartOffset = 0f;
	
	// Use this for initialization
	void Start () {
		door = transform.parent.GetComponentInChildren<DoorInteraction>();
		renderer = transform.parent.GetComponentInChildren<Renderer>();
		GameObject temp = GameObject.Find("Game Manager");		
		ghost = GameObject.Find("Ghost");
	}
	
	// Update is called once per frame
	void Update () {	
	}
	
	// This gameobject is set to Ignore Raycast Layer so that it doesn't affect the camera occlusion system
	// Therefore if we want to cast rays with a specific layer mask we need to add this layer manually to the mask if we want to ignore it
	void OnTriggerStay(Collider hit){
		if(SwitchMechanic_Script.getKidControl() == false)
		{
			if(hit.gameObject.tag == "Ghost" && door.hasAmulet == false && door.isUnusable == false)
			{			
				float clampedDistance = Mathf.Clamp(Mathf.Abs(Vector3.Distance(ghost.transform.position + ghost.transform.forward * fadeStartOffset, 
					transform.position)), 1, fadingRange);
				float alpha = Mathf.Lerp(0f, 1f, clampedDistance / fadingRange);
				//Debug.Log(alpha);
				renderer.material.SetColor("_Color", new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha));	
			}
		}
		else
			renderer.material.SetColor("_Color", new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1));
	}
}
