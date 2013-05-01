using UnityEngine;
using System.Collections;

public class HoverForward : MonoBehaviour {

public AudioClip hoverSound;
	
	[RPC]
	void networkplayHover()
	{
      if(networkView.isMine == true) { 
            networkView.RPC("networkplayHover", RPCMode.Others);
      }
      GetComponentInChildren<AudioSource>().PlayOneShot(hoverSound);
  }
	
	
	
}
