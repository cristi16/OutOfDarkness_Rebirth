using UnityEngine;
using System.Collections;

public class FlippableObject : PlaceableObject {
	
	public bool lookingRight;
	private Material mat;
	
	public void Awake()
	{
		mat = renderer.material;	
	}
	
	public override ObjectState GetObjectState ()
	{
		return new FlippableObjectState(id, lookingRight);
	}
	
	public override void PerformRightClick()
	{
		this.lookingRight = !lookingRight;
		if(lookingRight)
			mat.SetTextureScale("_MainTex", new Vector2(-1, 1));
		else
			mat.SetTextureScale("_MainTex", new Vector2(1, 1));
	}
}
