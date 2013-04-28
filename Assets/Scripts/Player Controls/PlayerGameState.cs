using UnityEngine;
using System.Collections;

public class PlayerGameState : MonoBehaviour {
	
	
	public int playerDurability = 100;
	public int playerHealth = 100;
	public int playerDroneCount = 10;
	public int resourcesHeld =0;
	private float timer = 0;
	public GUIText playerStatus;
	public GUITexture playerLifeBar;
	private float minPlayerLifeBarWidth = 0;
	private float maxPlayerLifeBarWidth = 200;
	public GUIText lifebarText;
	
	// Use this for initialization
	
	void Awake()
	{
		//playerStatus= GameObject.Find("PlayerStatus").guiText;
		//playerLifeBar= GameObject.Find("PlayerHealthBar").guiTexture;
		//lifebarText = GameObject.Find("ProgressType").guiText;
	}
	
	void Start () {
		playerDurability = playerDurability + 10*playerDroneCount;
		playerHealth = playerDurability;
		lifebarText.material.color = Color.black;
	}
	
	// Update is called once per frame
	void Update () {
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
			//Application.LoadLevel("Defeat");
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
		print("Resources: " + resourcesHeld);
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
}
