using UnityEngine;
using System.Collections;

public class MachineGunController : MonoBehaviour
{
	
	bool mShoot = false;
	bool mShotTimer = true;
	public int mDamage = 1;
	public float machineGunShotsPerSecond  = 12.5f;
	public GameObject gunBarrel;
	public GameObject mBullet;
	
	void MShotTimer()
	{
		mShotTimer = true;
	}
	
	[RPC]
	public void MsetClientShootingState (bool mShooting)
	{
		mShoot = mShooting;
	}
	
	public void Controls()
	{
		if (mShoot && mShotTimer)
		{
			Vector3 spawnPos = gunBarrel.transform.position + 
			gunBarrel.transform.forward.normalized*2.108931f+new Vector3(-.1f,-.1f,0);
		    GameObject prefab = Network.Instantiate(mBullet, spawnPos, 
			gunBarrel.transform.rotation, 2) as GameObject;
			prefab.rigidbody.AddForce (gunBarrel.transform.forward.normalized*18f, ForceMode.Impulse);
			mShotTimer = false;
			Invoke ("MShotTimer", 1.0f/machineGunShotsPerSecond);
			//NetworkView netView = gameObject.transform.FindChild("Machinegun").gameObject.networkView;
			//netView.RPC("networkplayMGun",RPCMode.All);
			RaycastHit hitInfo;
			var hit = Physics.Raycast (spawnPos, gunBarrel.transform.forward);
			Color color = hit ? Color.green : Color.red;
			Debug.DrawRay(spawnPos, gunBarrel.transform.forward*50f, color, 3f);
			if(Physics.Raycast(spawnPos,gunBarrel.transform.forward,out hitInfo))
			{
				if(hitInfo.transform.tag == "Enemy")
				{
					hitInfo.transform.gameObject.networkView.RPC ("damageEnemy", RPCMode.AllBuffered, mDamage);
				}
				else if(hitInfo.transform.tag == "Player")
				{
					hitInfo.transform.gameObject.networkView.RPC ("damagePlayer", RPCMode.AllBuffered, mDamage);
				}
				else if(hitInfo.transform.tag == "HasDrones")
				{
					hitInfo.transform.gameObject.networkView.RPC ("damageNode", RPCMode.AllBuffered, mDamage);
				}
			}
		}
	}
	
}
