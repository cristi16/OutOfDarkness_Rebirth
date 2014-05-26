using UnityEngine;
using System.Collections;

public class GameObjectAppearDisappearAction : Action {
	
	public override void execute(){		
		GetComponent<GameObjectDestroyer> ().Destroy ();
		
	}
}
