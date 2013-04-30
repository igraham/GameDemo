using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameServer : MonoBehaviour {
	
	public GameObject[] spawnPoints;
	public GameObject playerPrefab;
	public static string levelName;
	Vector3[] spawnLocations = new Vector3[4];
	int playerCount = 0;
	
	public List<ClientPlayerController> playerTracker = new List<ClientPlayerController>();
	public List<NetworkPlayer> scheduledSpawns = new List<NetworkPlayer>();
	
	public GameObject[] resourceNodes;
	public static ArrayList nodeScripts = new ArrayList();
	
	bool processSpawnRequests = false;
	//	Color[] playerColors = {Color.black, Color.blue, Color.green, Color.red};
	//Later on, use colors for team colors in spawning.
	void Awake()
	{
		enabled = Network.isServer;
		Application.runInBackground = true;
		for(int i = 0; i < spawnPoints.Length; i++)
		{
			spawnLocations[i] = spawnPoints[i].transform.position;
		}
		/*resourceNodes = GameObject.FindGameObjectsWithTag("ResourceNode");
		
		foreach(GameObject node in resourceNodes)
		{
			ResourceNodeScript nodeScript = (ResourceNodeScript) node.GetComponent(typeof(ResourceNodeScript));
			nodeScripts.Add(nodeScript);
		}*/
		//print(""+nodeScripts.Count);
		//scheduledSpawns.Add (Network.player);
		//processSpawnRequests = true;
		//requestSpawn (Network.player);
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log ("Spawning playerPrefab for new client");
		scheduledSpawns.Add (player);
		processSpawnRequests = true;
    }
	
	[RPC]
	void requestSpawn(NetworkPlayer requester)
	{
		if(Network.isClient)
		{
			Debug.LogError("Client tried to spawn itself! Revise logic!");
			return;
		}
		if(!processSpawnRequests)
		{
			return;
		}
		foreach(NetworkPlayer spawn in scheduledSpawns)
		{
			Debug.Log ("Checking player "+spawn.guid);
			if(spawn == requester)
			{
				//int num = int.Parse (spawn + "");
				GameObject handle = Network.Instantiate(playerPrefab, 
														spawnLocations[playerCount], 
														Quaternion.identity, 0) as GameObject;
				playerCount++;
				var sc = handle.transform.FindChild("NewTank").gameObject.GetComponent<ClientPlayerController>();
				if(!sc)
				{
					Debug.LogError("The prefab has no client player controller attached.");
				}
				playerTracker.Add (sc);
				NetworkView netView = handle.transform.FindChild("NewTank").gameObject.GetComponent<NetworkView>();
				netView.RPC ("setOwner", RPCMode.AllBuffered, spawn);
			}
		}
		scheduledSpawns.Remove (requester);
		if(scheduledSpawns.Count == 0)
		{
			Debug.Log ("spawns is empty! stopping spawn request processing");
			processSpawnRequests = false;
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log ("Player " +player.guid + " disconnected.");
		ClientPlayerController found = null;
		foreach(ClientPlayerController man in playerTracker)
		{
			if(man.getOwner() == player)
			{
				found = man;
				Network.RemoveRPCs (man.gameObject.transform.parent.networkView.viewID);
				Network.Destroy (man.gameObject.transform.parent.gameObject);
				playerCount--;
			}
		}
		if(found)
		{
			playerTracker.Remove (found);
		}
	}
}