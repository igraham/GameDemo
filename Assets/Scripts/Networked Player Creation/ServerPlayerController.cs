using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerPlayerController : MonoBehaviour
{
	bool shoot = false;
	bool mShoot = false;
	bool mShotTimer = true;
	bool shotTimer = true;
	public GameObject turret;
	public GameObject gunBarrel;
	public Camera playerCamera;
	public GameObject bullet;
	public GameObject mBullet;
	private GameObject radarDot;
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
	public MovementController movement;
	public TankTurretController turretRotation;
	
	void OnNetworkInstantiate (NetworkMessageInfo info)
	{
		radarDot = transform.FindChild("RadarDot").gameObject;
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
	void showRadarDotToEnemies()
	{
		radarDot.transform.localPosition = new Vector3(0, 67f, 0);
		Invoke("resetRadarDotPosition", 2.5f);
	}
	
	void resetRadarDotPosition()
	{
		radarDot.transform.localPosition = new Vector3(0, 57f, 0);
	}
	
	[RPC]
	void setClientShootingState (bool shooting)
	{
		shoot = shooting;
	}
	
	[RPC]
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
			Vector3 spawnPos = gunBarrel.transform.position + 
			gunBarrel.transform.forward.normalized*2.108931f+new Vector3(-.1f,-.1f,0);
		    GameObject prefab = Network.Instantiate(mBullet, spawnPos, 
			gunBarrel.transform.rotation, 0) as GameObject;
			prefab.rigidbody.AddForce (gunBarrel.transform.forward.normalized*18f, ForceMode.Impulse);
			Destroy (prefab, 5f);
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
	
	void FixedUpdate()
	{
		if (!isRespawning)
		{
			turretRotation.Controls();
			movement.Controls();
			shootingControls();
			MshootingControls();
		}
	}
}