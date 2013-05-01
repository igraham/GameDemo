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
	ResourceNodeScript node;
	PlayerGameState player;
	
	//All the GUI elements that need instantiating and assigning in scripts
	/*public GUIText playerStatus;
	public GameObject playerLifeBar;*/ 
	
	
	
	public GUIText resourceCommandsText;
	
	
	void OnNetworkInstantiate (NetworkMessageInfo info)
	{
			/*Network.Instantiate(playerStatus,playerStatus.transform.position,playerStatus.transform.rotation,0);
			Network.Instantiate(resourceCommandsText,resourceCommandsText.transform.position,resourceCommandsText.transform.rotation,0);
			Network.Instantiate(playerLifeBar,playerLifeBar.transform.position,playerLifeBar.transform.rotation,0);
			
			playerStatus.enabled = false;
			resourceCommandsText.enabled = false;
			playerLifeBar.SetActive(false);
			
			
			
			if(playerStatus.networkView.isMine)
				playerStatus.enabled = true;
			
			if(resourceCommandsText.networkView.isMine)
				resourceCommandsText.enabled = true;
			
			if(playerLifeBar.networkView.isMine)
				playerLifeBar.SetActive(true);
			
			PlayerGameState pGState = (PlayerGameState) GetComponent (typeof(PlayerGameState));
			pGState.playerStatus = this.playerStatus;
			pGState.playerLifeBar = playerLifeBar.transform.FindChild("ProgressBarFrame").guiTexture;
			pGState.lifebarText =  playerLifeBar.transform.FindChild("ProgressType").guiText;*/

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
			if(radar.GetComponent<AudioListener>())
			{
				radar.GetComponent<AudioListener>().enabled = false;
			}
			if(radar.GetComponent<GUILayer>())
			{
				radar.GetComponent<GUILayer>().enabled = false;
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
	
	void OnTriggerStay(Collider other) {
		
		if(node.isBusy ==false)
			resourceCommandsText.text = "Hit C to add a drone \n"+
										"Hit Z to remove a drone \n"+
										"Hit X to collect mined resources";	
	}
	
	void OnTriggerEnter(Collider other) {
		colliding = true;
		node = (ResourceNodeScript) other.collider.gameObject.GetComponent(typeof(ResourceNodeScript));
	}
	
	void OnTriggerExit(Collider other) {
		resourceCommandsText.text ="";
		colliding = false;
	}
}