using UnityEngine;
using System.Collections;

public class PlayerGameState : MonoBehaviour {
	
	
	public int playerDurability = 100;
	public int playerHealth = 100;
	public int playerDroneCount = 10;
	public int resourcesHeld =0;
	public int totalResources = 0;
	private float timer = 0;
	public GUIText playerStatus;
	public GUITexture playerLifeBar;
	private float minPlayerLifeBarWidth = 0;
	private float maxPlayerLifeBarWidth = 200;
	public GUIText lifebarText; 
	public GameObject droppedResources;
	float dropTimer = 0f;
	bool drop = false;
	int resourcesDropped =0;
	Vector3 v;
	public GUIText gameState;
	NetworkPlayer n;
	ClientPlayerController cpc;
	GameObject spawnLocation;
	bool bankResources = false;
	
	// Use this for initialization
	
	void Awake()
	{
		//playerStatus= GameObject.Find("PlayerStatus").guiText;
		//playerLifeBar= GameObject.Find("PlayerHealthBar").guiTexture;
		//lifebarText = GameObject.Find("ProgressType").guiText;
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		float dist = float.MaxValue;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("PlayerSpawn"))
		{
			if (Vector3.Distance (obj.transform.position, transform.position) < dist)
			{
				dist = Vector3.Distance (obj.transform.position, transform.position);
				spawnLocation = obj;
			}
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.tag.Equals ("PlayerSpawn") && col.gameObject.Equals(spawnLocation))
		{
			bankResources = true;
		}
	}
	
	void Start ()
	{
		playerDurability = playerDurability + 10*playerDroneCount;
		playerHealth = playerDurability;
		lifebarText.material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(bankResources)
		{
			collectResources();
			bankResources = false;
		}
		playerStatus.text = "Resources: "+ resourcesHeld + " Drones: "+ playerDroneCount;
		
		playerLifeBar.pixelInset = new Rect(playerLifeBar.pixelInset.x,
						playerLifeBar.pixelInset.y,(maxPlayerLifeBarWidth - minPlayerLifeBarWidth) * 
						((float)playerHealth/(float)playerDurability),playerLifeBar.pixelInset.height);
		
		timer += Time.deltaTime;
		//heal over time
		if(playerHealth < playerDurability && timer > 5.0f)
		{
			timer=0;
			playerHealth = playerHealth+playerDroneCount;
		}
		
		if(playerHealth > playerDurability)
			playerHealth = playerDurability;
		
		//game over
		if(playerHealth <= 0)
		{
			v = transform.position;
			networkView.RPC("respawnPlayer",RPCMode.Server);
			playerHealth = playerDurability;
			resourcesDropped = resourcesHeld;
			resourcesHeld = 0;
			drop = true;
		}
		
		if(drop)
		{
			dropTimer+= Time.deltaTime;
			setOwnership();
			if(dropTimer > 3.0f && n == Network.player)
			{
				GameObject dr = Network.Instantiate(droppedResources,v,Quaternion.identity,0) as GameObject;
				CollectDroppedResource cdr = (CollectDroppedResource) dr.GetComponent(typeof(CollectDroppedResource));
				cdr.setResourceAmount(resourcesDropped/2);
				drop = false;
				dropTimer =0;	
			}
		}
	}
	
	[RPC]
	public void playerAddDrone()
	{
		playerDroneCount++;
		playerDurability += 10;
		print("Drone Count: " + playerDroneCount);
	}
	
	[RPC]
	public void addResourcesHeld(int amount)
	{
		resourcesHeld += amount;
		//print("Resources: " + resourcesHeld);
	}
	
	[RPC]
	public void playerRemoveDrone()
	{
		if(playerDroneCount > 0)
		{
			playerDroneCount--;
			playerDurability -= 10;
			print("Drone Count: " + playerDroneCount);
		}
	}
	
	[RPC]
	public void damagePlayer(int damage)
	{
		playerHealth -= damage;
		/*debugText.text = "Player "+Network.player.guid+" was damaged by: "+damage+"\n" +
			"New health is: "+playerHealth; 
		Instantiate (debugText);*/
	}
	
	public void setOwnership()
	{
		cpc = (ClientPlayerController)gameObject.GetComponent (typeof(ClientPlayerController));
		n = cpc.getOwner ();
	}
	
	public void collectResources()
	{
		totalResources += resourcesHeld;
		GameObject obj = GameObject.Find("NetManager");
		obj.networkView.RPC ("addResources", RPCMode.Server, resourcesHeld, networkView.viewID);
		resourcesHeld = 0;
	}
}
