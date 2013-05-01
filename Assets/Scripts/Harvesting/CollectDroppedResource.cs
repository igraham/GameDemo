using UnityEngine;
using System.Collections;

public class CollectDroppedResource : MonoBehaviour {

	public int resourceAmount =0;
	NetworkPlayer n;
	public GameObject tank;
	ClientPlayerController cpc;
	
	public void setResourceAmount(int amt)
	{
		resourceAmount = amt;
	}
	
	public int getResourceAmount()
	{
		return resourceAmount;
	}
	
	public void setOwnership()
		{
			
			cpc = (ClientPlayerController)tank.GetComponent (typeof(ClientPlayerController));
			n = cpc.getOwner ();
			
		}
	
	[RPC]
	public void destroy()
	{
		setOwnership();
		if(n == Network.player)
			Network.Destroy(gameObject);
	}
	
}
