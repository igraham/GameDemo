using UnityEngine;
using System.Collections;

public class MortarController : MonoBehaviour
{
	
	bool shoot = false;
	bool shotTimer = true;
	public GameObject gunBarrel;
	public GameObject bullet;
	public float mortarPowerTimer = 0f;
	
	void ShotTimer()
	{
		shotTimer = true;	
	}
	
	[RPC]
	public void setClientShootingState(bool shooting)
	{
		shoot = shooting;
	}
	
	public void Controls()
	{
		if(shoot && shotTimer && mortarPowerTimer <= 1.5f)
		{
			mortarPowerTimer += Time.deltaTime;
		}
		else if(!shoot && shotTimer && mortarPowerTimer > 0f)
		{
			if(mortarPowerTimer > 1.5f){mortarPowerTimer = 1.5f;}
			float mortarSpeed = 18f + (18f * mortarPowerTimer);
			GameObject prefab = Network.Instantiate(bullet, gunBarrel.transform.position + 
				gunBarrel.transform.forward.normalized*2.108931f, 
				Quaternion.identity, 2) as GameObject;
			prefab.rigidbody.AddForce (gunBarrel.transform.forward.normalized*mortarSpeed, ForceMode.Impulse);
			Destroy (prefab, 5f);
			shotTimer = false;
			Invoke ("ShotTimer", 2.0f);
			NetworkView netView = gameObject.transform.FindChild("mortarSound").gameObject.networkView;
			netView.RPC("networkplayMortar",RPCMode.All);
			mortarPowerTimer = 0f;
		}
	}
	
}
