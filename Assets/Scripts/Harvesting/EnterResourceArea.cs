using UnityEngine;
using System.Collections;

public class EnterResourceArea : MonoBehaviour {

	public GUIText resourceCommandsText;
	bool colliding = false;
	ResourceNodeScript node;
	PlayerGameState player;
	
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(colliding)
		{
			player = (PlayerGameState) gameObject.GetComponent(typeof(PlayerGameState));
			
			if(Input.GetButtonDown("AddDrone") && player.playerDroneCount > 0 && node.droneCount <= 10)
			{
				networkView.RPC("requestToAddDrone", RPCMode.Server, node.resourceNodeNumber);
				//node.addDrone();
				//player.removeDrone();
			}
			if(Input.GetButtonDown("SubtractDrone")&& node.droneCount > 0)
			{
				networkView.RPC("requestToTakeDrone", RPCMode.Server, node.resourceNodeNumber);
				//node.subtractDrone();
				//player.addDrone();
			}
			if(Input.GetButtonDown("requestToCollectResources"))
			{
				networkView.RPC("requestToCollectResources", RPCMode.Server, node.resourceNodeNumber);
				//player.addResourcesHeld(node.extractResources());
			}
		}
	}
	
	void OnTriggerStay(Collider other) {
		
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
