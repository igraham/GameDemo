using UnityEngine;
using System.Collections;

public class NetState : MonoBehaviour
{

	public float timeStamp;
	public Vector3 pos;
	public Quaternion rot;
	
	public NetState()
	{
		timeStamp = 0f;
		pos = Vector3.zero;
		rot = Quaternion.identity;
	}
	
	public NetState(float time, Vector3 pos, Quaternion rot)
	{
		timeStamp = time;
		this.pos = pos;
		this.rot = rot;
	}
	
}
