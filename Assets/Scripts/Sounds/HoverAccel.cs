using UnityEngine;
using System.Collections;

public class HoverAccel : MonoBehaviour {

	public AudioClip hoverAccelSound;
	
	[RPC]
	void networkplayAccel()
	{
      if(networkView.isMine == true) { 
            networkView.RPC("networkplayAccel", RPCMode.Others);
      }
      GetComponentInChildren<AudioSource>().PlayOneShot(hoverAccelSound);
  }
	
	
	
}
