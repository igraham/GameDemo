using UnityEngine;
using System.Collections;

public class NetState
{

	public double timeStamp;
	public Vector3 pos;
	public Quaternion rot;
	
	public NetState()
	{
		timeStamp = 0d;
		pos = Vector3.zero;
		rot = Quaternion.identity;
	}
	
	public NetState(double time, Vector3 pos, Quaternion rot)
	{
		timeStamp = time;
		this.pos = pos;
		this.rot = rot;
	}
	
}
