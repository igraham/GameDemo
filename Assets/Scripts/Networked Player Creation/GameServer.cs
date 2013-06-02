using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameServer : MonoBehaviour {

	public GameObject[] spawnPoints;
	public GameObject playerPrefab;
	public static string levelName;
	Vector3[] spawnLocations = new Vector3[4];
	int playerCount = 0;
	public int totalResources1=0;
 	public int totalResources2=0;
	public int totalResources3=0;
	public int totalResources4=0;
	public string [] playerIDs = new string[4];
	public List<ClientPlayerController> playerTracker = new List<ClientPlayerController>();
	public List<NetworkPlayer> scheduledSpawns = new List<NetworkPlayer>();
	public int win = 100;

	public GameObject[] resourceNodes;
	public static ArrayList nodeScripts = new ArrayList();

	bool processSpawnRequests = false;

	void Awake()
	{
		enabled = Network.isServer;

		Application.runInBackground = true;
		for(int i = 0; i < spawnPoints.Length; i++)
		{
			spawnLocations[i] = spawnPoints[i].transform.position;
		}
	}

	void Update()
	{
		if(totalResources1 >= 100)
		{
			networkView.RPC ("loadVictoryOrDefeat", RPCMode.AllBuffered, playerIDs[0]);
		}
		else if(totalResources2 >= 100)
		{
			networkView.RPC ("loadVictoryOrDefeat", RPCMode.AllBuffered, playerIDs[1]);
		}
		else if(totalResources3 >= 100)
		{
			networkView.RPC ("loadVictoryOrDefeat", RPCMode.AllBuffered, playerIDs[2]);
		}
		else if(totalResources4 >= 100)
		{
			networkView.RPC ("loadVictoryOrDefeat", RPCMode.AllBuffered, playerIDs[3]);
		}
	}

	void OnLevelWasLoaded(int level)
	{
		if(level == 3)
		{
			List<NetworkPlayer> playerList = new List<NetworkPlayer>();
			foreach(NetworkPlayer player in Network.connections)
			{
				playerList.Add (player);
			}
			playerList.Sort ((x,y) => string.Compare(x.guid, y.guid));
			foreach(NetworkPlayer player in playerList)
			{
				scheduledSpawns.Add (player);
			}
			processSpawnRequests = true;
		}
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
				GameObject handle = Network.Instantiate(playerPrefab, 
														spawnLocations[playerCount], 
														Quaternion.identity, 1) as GameObject;
				playerIDs[playerCount] = ""+networkView.viewID;
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
	
	[RPC]
	public void addResources(int amount, NetworkViewID viewID)
	{
		string viewid = ""+viewID;
		if(viewid == playerIDs[0])
		{
			totalResources1 += amount;
		}
		if(viewid == playerIDs[1])
		{
			totalResources2 += amount;
		}
		if(viewid == playerIDs[2])
		{
			totalResources3 += amount;
		}
		if(viewid == playerIDs[3])
		{
			totalResources4 += amount;
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
