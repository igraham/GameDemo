using UnityEngine;
using System.Collections;

public class SplashController : MonoBehaviour {


    string gameID = "GGC 4650 Salvage of Eden";
	public GUIStyle myStyle = new GUIStyle();

	void Awake()
	{
		Application.runInBackground = true;
	}

	void OnServerInitialized()
	{
		//tell all clients to load level 1
		string levelName = "Lobby";
		networkView.RPC("clientLoadLevel", RPCMode.OthersBuffered, levelName);
		//load level 1
		Application.LoadLevel(levelName);
	}
	
	[RPC]
	void clientLoadLevel(string name)
	{
		//stop network processing until level is loaded.
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(name);
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
	 		Application.LoadLevel("Lobby");
		}
	}


	void OnGUI()
	{
		float w = 600;
		float h = 20;
		float x = (Screen.width - w)/2;
		float y = (Screen.height - h)/2;
		//Create buttons for server / client startup.
		if(GUI.Button(new Rect(x, y+=h+10, w, h), "Start Game Server",myStyle))
		{
			Network.InitializeServer(4, 40421, !Network.HavePublicAddress());
			var today = System.DateTime.Now;
			MasterServer.RegisterHost(gameID, "Salvage of Eden - "+today.ToString("yyyy-MM-dd_HH:mm:ss"));
		}

		if(GUI.Button(new Rect(x, y+=h+10, w, h), "Search for Servers",myStyle))
		{
			MasterServer.RequestHostList(gameID);
		}

		foreach(HostData host in MasterServer.PollHostList())
		{
			if(GUI.Button(new Rect(x, y+=h+10, w-50, h), "Join " + host.gameName,myStyle))
			{
				Network.Connect(host);
			}
		}
	}
}
