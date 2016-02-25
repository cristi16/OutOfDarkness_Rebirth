using UnityEngine;
using System.Collections;

public class GameObjectAppearDisappearAction : ActionOOD {
	
	public override void execute(){		
		GetComponent<GameObjectDestroyer> ().Destroy ();
		
	}
}
