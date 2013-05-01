using UnityEngine;
using System.Collections;

public class ClientPlayerController : MonoBehaviour
{
	bool f = false;
	bool r = false;
	bool rotR = false;
	bool rotL = false;
	bool strR = false;
	bool strL = false;
	float mouseX = 0;
	float mouseY = 0;
	NetworkPlayer owner;
	bool shooting = false;
	
	bool colliding = false;
	bool collect = false;
	int amount= 0;
	ResourceNodeScript node;
	PlayerGameState player;
	CollectDroppedResource cdr;
	GameObject dR;
	float timer = 0f;
	
	public GUIText resourceCommandsText;
	
	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		//may need in the future?
	}
	
	[RPC]
	void setOwner(NetworkPlayer player)
	{
		Debug.Log ("Setting the owner.");
		owner = player;
		if(player == Network.player)
		{
			enabled = true;
		}
		else
		{
			GameObject radar = gameObject.transform.FindChild("Radar").gameObject;
			if(radar.GetComponent<Camera>())
			{
				radar.GetComponent<Camera>().enabled = false;
			}
			if(gameObject.GetComponentInChildren<Camera>())
			{
				gameObject.GetComponentInChildren<Camera>().enabled = false;
			}
			if(gameObject.GetComponentInChildren<AudioListener>())
			{
				gameObject.GetComponentInChildren<AudioListener>().enabled = false;
			}
			if(gameObject.GetComponentInChildren<GUILayer>())
			{
				gameObject.GetComponentInChildren<GUILayer>().enabled = false;
			}
			if(transform.parent.transform.FindChild("HUDElements") != null)
			{
				GameObject hud = transform.parent.transform.FindChild("HUDElements").gameObject;
				
				if(hud.GetComponentInChildren<Camera>())
				{
					hud.GetComponentInChildren<Camera>().enabled = false;
				}
				if(hud.GetComponentInChildren<AudioListener>())
				{
					hud.GetComponentInChildren<AudioListener>().enabled = false;
				}
				if(hud.GetComponentInChildren<GUILayer>())
				{
					hud.GetComponentInChildren<GUILayer>().enabled = false;
				}
				
				Component[] hudTexts = hud.GetComponentsInChildren<GUIText> ();
				foreach (GUIText text in hudTexts) {
					text.enabled = false;
				}
				
				Component[] hudTextures = hud.GetComponentsInChildren<GUITexture> ();
				foreach (GUITexture texture in hudTextures) {
					texture.enabled = false;
				}
			}
		}
	}
	
	void Awake()
	{
		if(Network.isClient)
		{
			enabled = false;
		}
	}
	
	[RPC]
	public NetworkPlayer getOwner()
	{
		return owner;
	}
		
	void FixedUpdate ()
	{
		if(Network.player == owner)
		{
			bool forward = Input.GetButton("Forward");
			bool reverse = Input.GetButton("Reverse");
			bool rotateRight = Input.GetButton("Right");
			bool rotateLeft = Input.GetButton("Left");
			bool strRight = Input.GetButton("StrRight");
			bool strLeft = Input.GetButton("StrLeft");
			float mouseH = Input.GetAxis("Mouse X");
			float mouseV = Input.GetAxis("Mouse Y");
			bool shoot = Input.GetButton("Fire1") && Input.mousePosition.y < Screen.height -50;
			
			if(f!=forward || r!=reverse || rotR!=rotateRight || rotL!=rotateLeft 
				|| strR!=strRight || strL!=strLeft)
			{
				//RPC to server to send client movement controls input
				networkView.RPC("setClientMovementControls", RPCMode.Server, forward, reverse, rotateRight, 
																			rotateLeft, strRight, strLeft);
			}
			if(mouseX!=mouseH || mouseY!=mouseV)
			{
				//RPC to server to send mouse controls (separate from movement, performance reasons)
				networkView.RPC("setClientTurretControls", RPCMode.Server, mouseH, mouseV);
			}
			if(shoot!=shooting)
			{
				//RPC to server to send mouse click for shooting.
				networkView.RPC("setClientShootingState", RPCMode.Server, shoot);
			}
			if(colliding && node.nodeMode ==0 && node.isBusy == false)
			{
				player = (PlayerGameState) gameObject.GetComponent(typeof(PlayerGameState));
			
				if(Input.GetButtonDown("AddDrone") && player.playerDroneCount > 0 && node.droneCount <= 5)
				{
					networkView.RPC("requestToAddDrone", RPCMode.Server, node.resourceNodeNumber);
				}
				if(Input.GetButtonDown("SubtractDrone")&& node.droneCount > 0)
				{
					networkView.RPC("requestToTakeDrone", RPCMode.Server, node.resourceNodeNumber);
				}
				if(Input.GetButtonDown("CollectResources"))
				{
					networkView.RPC("requestToCollectResources", RPCMode.Server, node.resourceNodeNumber);
				}
			}
			if(collect)
			{
				
				if(timer == 0.0f)
				{
					if(dR != null)
					{
						networkView.RPC("requestToCollectDropppedResources", RPCMode.Server, amount);
					
						cdr.networkView.RPC("destroy",RPCMode.AllBuffered);
					}
				}
				timer += Time.deltaTime;
					if(timer > 3.0f)
					{
						collect = false;
						timer = 0f;
					}
				
			}
			
			//store history
			f = forward;
			r = reverse;
			rotR = rotateRight;
			rotL = rotateLeft;
			strR = strRight;
			strL = strLeft;
			mouseX = mouseH;
			mouseY = mouseV;
			shooting = shoot;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		
		if(other.tag.Equals("DroppedResource"))
		{
		}	
		else
		{
			if(node.isBusy ==false)
				resourceCommandsText.text = "Hit C to add a drone \n"+
											"Hit Z to remove a drone \n"+
											"Hit X to collect mined resources";	
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("DroppedResource"))
		{
			collect = true;
			CollectDroppedResource collector = (CollectDroppedResource)other.GetComponent(typeof(CollectDroppedResource));
			amount = collector.getResourceAmount();
			cdr = (CollectDroppedResource) other.GetComponent(typeof(CollectDroppedResource));
			dR = other.gameObject;
		}	
		else
		{
			colliding = true;
			node = (ResourceNodeScript) other.collider.gameObject.GetComponent(typeof(ResourceNodeScript));
			
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag.Equals("DroppedResource"))
		{
		}	
		else
		{
			resourceCommandsText.text ="";
			colliding = false;
		}
	}
}