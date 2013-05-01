using UnityEngine;
using System.Collections;

public class MortarPlayer : MonoBehaviour {
	public AudioClip mortarSound;
	
	[RPC]
	void networkplayMortar()
	{
      if(networkView.isMine == true) { 
            networkView.RPC("networkplayMortar", RPCMode.Others);
      }
      GetComponentInChildren<AudioSource>().PlayOneShot(mortarSound);
  }
	
	
	
}
