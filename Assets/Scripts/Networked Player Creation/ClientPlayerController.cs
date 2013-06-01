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
	bool mShooting = false;
	bool colliding = false;
	bool collect = false;
	int amount = 0;
	ResourceNodeScript node;
	PlayerGameState player;
	CollectDroppedResource cdr;
	GameObject dR;
	float timer = 0f;
	public GUIText resourceCommandsText;
	Fading radarControl;
	public float radarCooldownTime = 5f;
	bool radarCooldown = false;
	bool isNodeBusy = false;
	public Vector3 serverPosition;
	public Quaternion serverRotation;
	public float serverVelocity;
	public float positionErrorThreshold = 0.2f;
	public float angleErrorThreshold = 0.5f;
	public float moveSpeed = 15f;
	public float rotationSpeed = 100f;
	public MovementController movement;

	public void lerpToTarget()
	{
		float distance = Vector3.Distance(transform.position, serverPosition);
		float angle = Quaternion.Angle (transform.rotation, serverRotation);

		float lerp = ((1f / distance) * moveSpeed) / ((1 / serverVelocity+0.01f) * 150f);
		float alerp = ((1 / distance) * rotationSpeed) / 20;

		if(distance >= positionErrorThreshold)
		{
			transform.position = Vector3.Lerp (transform.position, serverPosition, lerp);
		}
		if(angle >= angleErrorThreshold)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, serverRotation, alerp);
		}
	}
	
	private void RadarCooldown()
	{
		radarCooldown = false;
	}
	
	[RPC]
	void setOwner(NetworkPlayer player)
	{
		Debug.Log ("Setting the owner.");
		owner = player;
		
		if(player == Network.player)
		{
			enabled = true;
			GameObject radar = gameObject.transform.FindChild("Radar").gameObject;
			if(radar.transform.FindChild("RadarBlackout"))
			{
				radarControl = radar.transform.FindChild("RadarBlackout").GetComponent<Fading>();
			}
			if(Network.isClient)
			{
				GameObject serverCam = GameObject.Find("ServerCamera");
				if(serverCam)
				{
					if(serverCam.GetComponent<Camera>())
					{
						serverCam.GetComponent<Camera>().enabled = false;
					}
					if(serverCam.GetComponent<AudioListener>())
					{
						serverCam.GetComponent<AudioListener>().enabled = false;
					}
				}
			}
		}
		else if(Network.isServer || player != Network.player)
		{
			GameObject radar = gameObject.transform.FindChild("Radar").gameObject;
			GameObject radarBlackout = radar.transform.FindChild("RadarBlackout").gameObject;

			if(radar.GetComponent<Camera>())
			{
				radar.GetComponent<Camera>().enabled = false;
			}
			if(radarBlackout.GetComponent<MeshRenderer>())
			{
				radarBlackout.GetComponent<MeshRenderer>().enabled = false;
			}
			if(gameObject.GetComponentInChildren<Camera>())
			{
				gameObject.GetComponentInChildren<Camera>().enabled = false;
			}
			if(transform.parent.transform.FindChild("HUDElements") != null
				&& transform.parent.transform.FindChild("NewTank") != null)
			{
				GameObject hud = transform.parent.transform.FindChild("HUDElements").gameObject;
				GameObject tank = transform.parent.transform.FindChild("NewTank").gameObject;
				
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
				if(tank.GetComponentInChildren<Camera>())
				{
					tank.GetComponentInChildren<Camera>().enabled = false;
				}
				if(tank.GetComponentInChildren<AudioListener>())
				{
					tank.GetComponentInChildren<AudioListener>().enabled = false;
				}
				if(tank.GetComponentInChildren<GUILayer>())
				{
					tank.GetComponentInChildren<GUILayer>().enabled = false;
				}
				
				Component[] hudTexts = hud.GetComponentsInChildren<GUIText> ();
				foreach (GUIText text in hudTexts)
				{
					text.enabled = false;
				}
				
				Component[] hudTextures = hud.GetComponentsInChildren<GUITexture> ();
				foreach (GUITexture texture in hudTextures)
				{
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
		if(Network.isServer){return;}
		
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

			bool shoot = Input.GetMouseButton(0) && Input.mousePosition.y < Screen.height -50;
			bool mShoot = Input.GetMouseButton(1) && Input.mousePosition.y < Screen.height -50;
			
			if(!radarCooldown && Input.GetKey(KeyCode.Space))
			{
				radarControl.FadeRadar();
				radarCooldown = true;
				Invoke("RadarCooldown",radarCooldownTime);
				networkView.RPC("showRadarDotToEnemies", RPCMode.Server);
			}
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
				//RPC to server to send mouse click for when the left click is released.
				networkView.RPC("setClientShootingState", RPCMode.Server, shoot);
			}

			if(mShoot!=mShooting)
			{
				//RPC to server to send mouse click for shooting.
				networkView.RPC("MsetClientShootingState", RPCMode.Server, mShoot);
			}
			if(colliding && isNodeBusy == false)
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
			mShooting = mShoot;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		
		if(other.tag.Equals("DroppedResource"))
		{
		}	
		else if(other.tag.Equals("PlayerSpawn"))
		{
			
		}
		else
		{
			node = (ResourceNodeScript) other.collider.gameObject.GetComponent(typeof(ResourceNodeScript));
			if( node.nodeMode == 0 && node.isBusy == false)
			{
				isNodeBusy = false;
			}
			else
			{
				isNodeBusy = true;
			}
			if(node.isBusy == false)
			{
				resourceCommandsText.text = "Hit C to add a drone \n"+
											"Hit Z to remove a drone \n"+
											"Hit X to collect mined resources";
			}
		}
	}
	
	[RPC]
	void loadVictoryOrDefeat(string viewid)
	{
		if((""+networkView.viewID)==viewid)
		{
			Application.LoadLevel("victorytwo");
		}
		else
		{
			Application.LoadLevel("Defeat");
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
		else if(other.tag.Equals("PlayerSpawn"))
		{
			
		}
		else
		{
			colliding = true;
			node = (ResourceNodeScript) other.collider.gameObject.GetComponent(typeof(ResourceNodeScript));
			if(node.nodeMode == 0 && node.isBusy == false)
			{
				isNodeBusy = false;
			}
			else
			{
				isNodeBusy = true;
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag.Equals("DroppedResource"))
		{
			
		}
		else if(other.tag.Equals("PlayerSpawn"))
		{
			
		}
		else
		{
			resourceCommandsText.text ="";
			colliding = false;
		}
	}
}