using UnityEngine;
using System.Collections;

public class MotorShot : MonoBehaviour {
	
	public GameObject bullet;
	public GameObject turret;
	public GameObject gunBarrel;
	public GameObject tankBody;
	public float power = 5f;
	bool shotTimer = true;
	public float timer = 0.0f; 
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	[RPC]
	void Update () 
	{
		while(Input.GetButtonDown("Fire1"))
		{
			timer++;
			print("timer is " + timer);
		}
		
		//if(Input.GetButtonUp("Fire1") && Input.mousePosition.y < Screen.height -50)
		//{
		//	if(shotTimer)
		//	{
		//		GameObject prefab = Instantiate(bullet, transform.position+new Vector3(0,-.25f,0), 
		//				Quaternion.identity) as GameObject;
		//		Physics.IgnoreCollision(prefab.collider, collider);
		//		Physics.IgnoreCollision(prefab.collider, turret.collider);
		//		Physics.IgnoreCollision(prefab.collider, tankBody.collider);
		//		Physics.IgnoreCollision(prefab.collider, gunBarrel.collider);
	    //        prefab.rigidbody.AddForce(transform.forward*power*timer,ForceMode.Impulse);
		//		Destroy(prefab,5f);
		//	    shotTimer=false;
		//		Invoke("ShotTimer",.5f);
		//	}
		//}
	}
	//void ShotTimer()
	//{
	//	shotTimer=true;	
	//}
}
