using UnityEngine;
using System.Collections;

public class Predictor : MonoBehaviour
{
	
	public Rigidbody observedRigidbody;
	public ClientPlayerController receiver;
	public float pingMargin = 0.5f;
	
	private float clientPing;
	private NetState[] serverStateBuffer = new NetState[20];
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 pos = observedRigidbody.position;
		Quaternion rot = observedRigidbody.rotation;
		float vel = observedRigidbody.velocity.magnitude;
		
		if(stream.isWriting)
		{
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref vel);
		}
		else
		{
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref vel);
			receiver.serverPosition = pos;
			receiver.serverRotation = rot;
			receiver.serverVelocity = vel;
			
			receiver.lerpToTarget();
			
			for(int i = serverStateBuffer.Length - 1; i >= 1; i--)
			{
				serverStateBuffer[i] = serverStateBuffer[i-1];
			}
			
			serverStateBuffer[0] = new NetState(info.timestamp, pos, rot);
		}
	}
	
	void Update()
	{
		if((Network.player == receiver.getOwner()) || Network.isServer)
		{
			return;
		}
		
		clientPing = (float)(Network.GetAveragePing(Network.connections[0])/100) + pingMargin;
		double interpolationTime = Network.time - clientPing;
		
		if(serverStateBuffer[0] == null)
		{
			serverStateBuffer[0] = new NetState(0, transform.position, transform.rotation);
		}
		
		if(serverStateBuffer[0].timeStamp > interpolationTime)
		{
			for(int i = 0; i < serverStateBuffer.Length; i++)
			{
				if(serverStateBuffer[i] == null)
				{
					continue;
				}

				if(serverStateBuffer[i].timeStamp <= interpolationTime
					|| i == serverStateBuffer.Length - 1)
				{
					NetState bestTarget = serverStateBuffer[Mathf.Max (i-1, 0)];
					NetState bestStart = serverStateBuffer[i];
					
					float timeDiff = (float)(bestTarget.timeStamp - bestStart.timeStamp);
					float lerpTime = 0f;
					
					if(timeDiff > 0.0001d)
					{
						lerpTime = (float)((interpolationTime - bestStart.timeStamp) / timeDiff);
					}
					
					transform.position = Vector3.Lerp(bestStart.pos, bestTarget.pos, lerpTime);
					transform.rotation = Quaternion.Slerp(bestStart.rot, bestTarget.rot, lerpTime);
					return;
				}
			}
		}
		else
		{
			NetState latest = serverStateBuffer[0];
			transform.position = Vector3.Lerp(transform.position, latest.pos, 0.5f);
			transform.rotation = Quaternion.Slerp(transform.rotation, latest.rot, 0.5f);
		}
	}
}
