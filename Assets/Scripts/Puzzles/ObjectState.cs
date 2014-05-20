using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectState : MonoBehaviour
{		
	public int id;
	
	public ObjectState(int id)
	{
		this.id = id;	
	}
	
	public virtual bool IsEqualTo(ObjectState comp)
	{
		if(id == comp.id)
			return true;
		else
			return false;
	}
	
}
