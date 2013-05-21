using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerPlayerController : MonoBehaviour
{
	public float speed = 5f;
	public float maxSpeed = 15f;
	public Vector3 rotationSpeed = new Vector3 (0, 100f, 0);
	bool forward = false;
	bool reverse = false;
	bool rotateRight = false;
	bool rotateLeft = false;
	bool strRight = false;
	bool strLeft = false;
	float mouseH = 0;
	float mouseV = 0; 
	bool shoot = false;
	bool mShoot = false;
	bool mShotTimer = true;
	bool shotTimer = true;
	public GameObject turret;
	public GameObject gunBarrel;
	public Camera playerCamera;
	public GameObject bullet;
	public GameObject mBullet;
	GameObject spawnLocation;
	bool isRespawning = false;
	public int mDamage = 1;
	public GameObject[] resourceNodes;
	public float machineGunShotsPerSecond  = 12.5f;
	public float mortarPowerTimer = 0f;
	//public static ArrayList nodeScripts = new ArrayList();
	public Dictionary<int,GameObject> sortedNodeList = new Dictionary<int,GameObject> ();
	Color[] tankColor = new Color[]{Color.red,Color.blue,Color.green,Color.yellow};
	public GameObject[] pieceChange;
	
	void OnNetworkInstantiate (NetworkMessageInfo info)
	{
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("PlayerSpawn");
		//Finds all the nodes
		//print ("Test Run1");
		resourceNodes = GameObject.FindGameObjectsWithTag ("ResourceNode");
		foreach (GameObject node in resourceNodes)
		{
			ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
			sortedNodeList.Add (nodeScript.resourceNodeNumber, node);
			
		}
		
		//Sorts them by number as key from when the server initialized
		
		if (resourceNodes.Length == 0)
			print ("Empty");
		float dist = float.MaxValue;
		foreach (GameObject obj in spawns)
		{
			if (Vector3.Distance (obj.transform.position, transform.position) < dist)
			{
				dist = Vector3.Distance (obj.transform.position, transform.position);
				spawnLocation = obj;
			}
		}
		
		//Change the color of the tank's main pieces
		Color c;
		for (int i = 0; i< spawns.Length; i++)
		{
			if (spawns [i] == spawnLocation)
			{
				c = tankColor [i];
				foreach (GameObject obj in pieceChange)
				{
					obj.renderer.material.color = c;
				}
			}
		}	
	}

	void Start ()
	{
		if(Network.isServer)
		{
			if(gameObject.GetComponentInChildren<GUILayer>())
			{
				gameObject.GetComponentInChildren<GUILayer>().enabled = false;
			}
		}
	}
	
	[RPC]
	void setClientTurretControls (float mouseX, float mouseY)
	{
		mouseH = mouseX;
		mouseV = mouseY;
	}
	
	[RPC]
	void setClientMovementControls (bool f, bool r, bool rotR, bool rotL, bool strR, bool strL)
	{
		forward = f;
		reverse = r;
		rotateRight = rotR;
		rotateLeft = rotL;
		strRight = strR;
		strLeft = strL;
	}
	
	[RPC]
	void setClientShootingState (bool shooting)
	{
		shoot = shooting;
	}
	
	void MsetClientShootingState (bool mShooting)
	{
		mShoot = mShooting;
	}
	
	[RPC]
	void respawnPlayer ()
	{
		transform.position = spawnLocation.transform.position;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		playerCamera.transform.rotation = Quaternion.identity;
		turret.transform.rotation = Quaternion.identity;
		gunBarrel.transform.rotation = Quaternion.identity;
		isRespawning = true;
		Invoke ("doneRespawning", 3f);
	}
	
	[RPC]
	void requestToAddDrone(int nodeNumber)
	{
		if(sortedNodeList.ContainsKey(nodeNumber))
		{	
			ResourceNodeScript nodeScript = (ResourceNodeScript)sortedNodeList [nodeNumber].gameObject.GetComponent (typeof(ResourceNodeScript));
			if(nodeScript.droneCount < 5)
			{
				sortedNodeList [nodeNumber].networkView.RPC ("addDrone", RPCMode.AllBuffered);
				if(nodeScript.isNode == false)
				{
					GUIText nodeText1 = transform.parent.transform.FindChild("HUDElements").transform.FindChild("NodeTexts").transform.FindChild("NodeText1").guiText;
					NodeGameState nGState = (NodeGameState)nodeText1.GetComponent(typeof(NodeGameState));
				
					nGState.networkView.RPC ("addNode",RPCMode.AllBuffered,nodeNumber);
				}
				//PlayerGameState player = (PlayerGameState) gameObject.GetComponent(typeof(PlayerGameState));
				networkView.RPC ("playerRemoveDrone", RPCMode.AllBuffered);
			}
		}
	}
	
	[RPC]
	void requestToTakeDrone(int nodeNumber)
	{
		if(sortedNodeList.ContainsKey (nodeNumber))
		{
			sortedNodeList [nodeNumber].networkView.RPC ("removeDrone", RPCMode.AllBuffered);
			ResourceNodeScript nodeScript = (ResourceNodeScript)sortedNodeList [nodeNumber].gameObject.GetComponent (typeof(ResourceNodeScript));
			if(nodeScript.droneCount > 0)
				{
				sortedNodeList [nodeNumber].networkView.RPC ("removeDrone", RPCMode.AllBuffered);
				if(nodeScript.droneCount <= 0)
				{
					GUIText nodeText1 = transform.parent.transform.FindChild("HUDElements").transform.FindChild("NodeTexts").transform.FindChild("NodeText1").guiText;
					NodeGameState nGState = (NodeGameState)nodeText1.GetComponent(typeof(NodeGameState));
					nGState.networkView.RPC ("removeNode",RPCMode.AllBuffered,nodeNumber);
				}
			
				networkView.RPC ("playerAddDrone", RPCMode.AllBuffered);
			}
		}
	}
	
	[RPC]
	void requestToCollectResources(int nodeNumber)
	{
		if (sortedNodeList.ContainsKey (nodeNumber))
		{
			sortedNodeList [nodeNumber].networkView.RPC ("extractResources", RPCMode.AllBuffered);
			ResourceNodeScript nodeScript = (ResourceNodeScript)sortedNodeList [nodeNumber].gameObject.GetComponent (typeof(ResourceNodeScript));
			//player.addResourcesHeld(sortedNodeList[nodeNumber].networkView.RPC("extractResources",RPCMode.AllBuffered));
			networkView.RPC ("addResourcesHeld", RPCMode.AllBuffered, nodeScript.extractable);
		}
	}
	
	[RPC]
	void requestToCollectDropppedResources(int amt)
	{
		networkView.RPC ("addResourcesHeld", RPCMode.AllBuffered, amt);
		
	}
	
	private void doneRespawning()
	{
		isRespawning = false;
	}
	
	private void turretControls()
	{
		//------------------turret----------------------//
		float upDown = -mouseV * Time.deltaTime * 60f;
		float leftRight = mouseH * Time.deltaTime * 60f;
		
		//making it turn up or down
		if(gunBarrel.transform.localEulerAngles.x < 2)
		{
			gunBarrel.transform.Rotate (upDown, 0, 0);
		}
		else if(gunBarrel.transform.localEulerAngles.x > 335)
		{
			gunBarrel.transform.Rotate (upDown, 0, 0);         
		}
		
		//making it turn left or right
		if(turret.transform.localEulerAngles.y < 33)
		{
			turret.transform.Rotate(0, leftRight, 0);
		}
		else if(turret.transform.localEulerAngles.y > 327)
		{
			turret.transform.Rotate(0, leftRight, 0);        
		}
 
		//making it not exeed rotation limit (left and right) and preventing it lock up
		if(turret.transform.localEulerAngles.y >= 33 && turret.transform.localEulerAngles.y < 327)
		{
			if(turret.transform.localEulerAngles.y < 180)
			{
				turret.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 32.9F, 0);
				playerCamera.transform.Rotate (0, leftRight, 0);
			}
			else
			{
				turret.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 327.1F, 0);
				playerCamera.transform.Rotate (0, leftRight, 0);
			}
		}
 
		//making it not exeed rotation limit (up and down) and preventing it lock up
		if(gunBarrel.transform.localEulerAngles.x >= 2 && gunBarrel.transform.localEulerAngles.x < 335)
		{
			if(gunBarrel.transform.localEulerAngles.x < 180)
			{
				gunBarrel.transform.localEulerAngles = new Vector3(1.9F, gunBarrel.transform.localEulerAngles.y, 0);
			}
			else
			{
				gunBarrel.transform.localEulerAngles = new Vector3(335.1F, gunBarrel.transform.localEulerAngles.y, 0);
			}
		}
 
		//making sure it doesn't turn on it's z axis
		if(turret.transform.localEulerAngles.z != 0)
		{
			turret.transform.localEulerAngles = new Vector3(turret.transform.localEulerAngles.x, 
				turret.transform.localEulerAngles.y, 0);
		}
		//------------------turret----------------------//
	}
	
	private void movementControls()
	{
		//------------------movement--------------------//
		//move forwards
		if(forward)
		{
			rigidbody.AddForce(transform.forward.normalized * speed);
			if(rigidbody.velocity.magnitude > maxSpeed)
			{
				rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
			}
			rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, 
												  		  turret.transform.rotation, 
												  		  Time.deltaTime * 4f);
			turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, 
														 		 rigidbody.rotation, 
														 		 Time.deltaTime * 2f);
			playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, 
															   		   turret.transform.rotation, 
															   		   Time.deltaTime * 3f);
			
		}
		//move backwards
		if(reverse)
		{
			rigidbody.AddForce(-1f * transform.forward.normalized * speed);
			if(rigidbody.velocity.magnitude > maxSpeed)
			{
				rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
				
			}
		}
		//rotate right
		if(rotateRight)
		{
			Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime);
			rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
		}
		//rotate left
		if(rotateLeft)
		{
			Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime * -1f);
			rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
		}
		//strafe right
		if(strRight)
		{
			rigidbody.AddForce(transform.right.normalized * speed);
			if(rigidbody.velocity.magnitude > maxSpeed)
			{
				rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
			}
		}
		//strafe left
		if(strLeft)
		{
			rigidbody.AddForce(-1f * transform.right.normalized * speed);
			if(rigidbody.velocity.magnitude > maxSpeed)
			{
				rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
			}
		}
		//------------------movement--------------------//
	}
	
	void ShotTimer()
	{
		shotTimer = true;	
	}
	
	void MShotTimer()
	{
		mShotTimer = true;
	}
	
	private void shootingControls()
	{
		if(shoot && shotTimer && mortarPowerTimer <= 1.5f)
		{
			mortarPowerTimer += Time.deltaTime;
		}
		else if(!shoot && shotTimer && mortarPowerTimer > 0f)
		{
			if(mortarPowerTimer > 1.5f){mortarPowerTimer = 1.5f;}
			float mortarSpeed = 18f + (18f * mortarPowerTimer);
			print ("motarSpeed is " + mortarSpeed);
			GameObject prefab = Network.Instantiate(bullet, gunBarrel.transform.position + 
				gunBarrel.transform.forward.normalized*2.108931f, 
				Quaternion.identity, 0) as GameObject;
			prefab.rigidbody.AddForce (gunBarrel.transform.forward.normalized*mortarSpeed, ForceMode.Impulse);
			Destroy (prefab, 5f);
			shotTimer = false;
			Invoke ("ShotTimer", 2.0f);
			NetworkView netView = gameObject.transform.FindChild("mortarSound").gameObject.networkView;
			netView.RPC("networkplayMortar",RPCMode.All);
			mortarPowerTimer = 0f;
		}
	}
	
	private void MshootingControls ()
	{
		if (mShoot && mShotTimer)
		{
		    GameObject prefab = Network.Instantiate(mBullet, gunBarrel.transform.position + 
			gunBarrel.transform.forward.normalized*2.108931f+new Vector3(-.1f,-.1f,0), 
			gunBarrel.transform.rotation, 0) as GameObject;
			prefab.rigidbody.AddForce (gunBarrel.transform.forward.normalized*18f, ForceMode.Impulse);
			Destroy (prefab, 5f);
			mShotTimer = false;
			Invoke ("MShotTimer", 1.0f/machineGunShotsPerSecond);
			NetworkView netView = gameObject.transform.FindChild("Machinegun").gameObject.networkView;
			netView.RPC("networkplayMGun",RPCMode.All);
			RaycastHit hitInfo;
			if(Physics.Raycast(transform.position,transform.forward,out hitInfo))
			{
				 if(hitInfo.transform.tag== "Enemy")
				{
					hitInfo.transform.gameObject.networkView.RPC ("damageEnemy", RPCMode.AllBuffered, mDamage);
				}
				else if(hitInfo.transform.tag== "Player")
				{
					hitInfo.transform.gameObject.networkView.RPC ("damagePlayer", RPCMode.AllBuffered, mDamage);
				}
			}
		}
	}
	
	void FixedUpdate()
	{
		if (!isRespawning)
		{
			turretControls();
			movementControls();
			shootingControls();
			MshootingControls();
		}
	}
}