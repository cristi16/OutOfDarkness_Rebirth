using UnityEngine;
using System.Collections;

public class KidPuzzleFriend : ActionOOD {

	public GameObjectDestroyer god;
	public GameObjectEnabler goe;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void execute(){
		Destroy (GetComponent<TextTrigger> ());
		goe.Enable ();
		god.Destroy ();

		if (transform.childCount>0) {
			Transform aux = transform.GetChild(0);
			aux.parent = null;
			transform.parent = aux;
			transform.localPosition = new Vector3 (0f, 0f, 0f);
			GetComponent<InteractiveTrigger> ().triggerToActivate = transform.parent.GetComponent<TextTrigger> ();
		}
	}
}
