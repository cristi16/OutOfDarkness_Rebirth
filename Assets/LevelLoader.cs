using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator Start() {
	    AsyncOperation async = Application.LoadLevelAdditiveAsync(1);
	    yield return async;
	    Debug.Log("Loading complete");
    }
}
