using UnityEngine;
using System.Collections;

public class PuzzleEnablerDisabler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void EnablePuzzle(){
		Invoke("EnablePuzzleColliders",1f);
	}
	
	public void EnablePuzzleColliders(){
		transform.GetChild(0).gameObject.SetActive(true);
		GetComponentInChildren<InteractiveTrigger>().GetComponent<Collider>().enabled=true;
		GetComponentInChildren<InteractiveTrigger>().enabled=true;
		GetComponentInChildren<InteractiveCollider>().GetComponent<Collider>().enabled=true;
		GetComponentInChildren<InteractiveCollider>().enabled=true;	
		GetComponentInChildren<PuzzleController>().isActivated=false;
		transform.GetChild(0).gameObject.SetActive(false);
	}
}
