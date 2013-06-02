using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour {

	public GameObject[] resourceNodes;
	public static ArrayList nodeScripts = new ArrayList();


	void OnConnectedToServer()
	{
		Debug.Log ("Disabling message queue!");
		Network.isMessageQueueRunning = false;
		Network.SetSendingEnabled(0, false);
	}

	void OnLevelWasLoaded(int level)
	{
		//enable the message processing for clients now that level has been loaded
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
		if(level > 1 && level < 4 && Network.isClient)
		{
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
