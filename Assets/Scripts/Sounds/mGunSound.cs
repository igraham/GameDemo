using UnityEngine;
using System.Collections;

public class mGunSound : MonoBehaviour {

	public AudioClip gunSound;
	
	[RPC]
	void networkplayMGun()
	{
      if(networkView.isMine == true) { 
            networkView.RPC("networkplayMGun", RPCMode.Others);
      }
      GetComponentInChildren<AudioSource>().PlayOneShot(gunSound);
  }
}
