using UnityEngine;
using System.Collections;

[System.Serializable]
public class FlippableObjectState : ObjectState
{
	public bool lookingRight;
	
	public FlippableObjectState(int id, bool lookingRight) : base(id)
	{
		this.lookingRight = lookingRight;
	}
	
	public override bool IsEqualTo(ObjectState comp)
	{	
		FlippableObjectState temp = (FlippableObjectState) comp;
		if( (id == temp.id) && (lookingRight == temp.lookingRight))
			return true;
		else
			return false;
	}
}
