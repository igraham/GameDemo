using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour {
	
	public GameObject[] resourceNodes;
	public static ArrayList nodeScripts = new ArrayList();
	
	
	void OnConnectedToServer()
	{
		Debug.Log ("Disabling message queue!");
		Network.isMessageQueueRunning = false;
		Application.LoadLevel (GameServer.levelName);
		
		
	}
	
	void OnLevelWasLoaded(int level) {
		//enable the message processing for clients now that level has been loaded
		if(level != 0 && Network.isClient)
		{
			Network.isMessageQueueRunning = true;
			Debug.Log ("Level was loaded, requesting spawn");
			Debug.Log ("Re-enabling message queue!");
		
			networkView.RPC ("requestSpawn", RPCMode.Server, Network.player);
			//networkView.RPC ("setResourceNodeIndex",RPCMode.Server);
			resourceNodes = GameObject.FindGameObjectsWithTag("ResourceNode");
			
			foreach(GameObject node in resourceNodes)
			{
				ResourceNodeScript nodeScript = (ResourceNodeScript) node.GetComponent(typeof(ResourceNodeScript));
				nodeScripts.Add(nodeScript);
			}
			
		}
    }
}
