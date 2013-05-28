using UnityEngine;
using System.Collections;

public class LobbyController : MonoBehaviour {
	
	public int maxNumberOfPlayers = 4;
	public bool autoStart = false;
	private int playersInLobby = 0;
	
	void OnGUI()
	{
		if(Network.isServer) //don't let the clients start the game early!
		{
			float w = 600;
			float h = 20;
			float x = (Screen.width - w)/2;
			float y = (Screen.height - h)/2;
			if(GUI.Button (new Rect(x, y+=h+10, w, h),"Load Game"))
			{
				networkView.RPC ("loadGame",RPCMode.AllBuffered);
				Network.isMessageQueueRunning = true;
        		Network.SetSendingEnabled(0, true);
			}
		}
	}
	
	[RPC]
	public void loadGame()
	{
		Network.SetSendingEnabled(0, false);
        Network.isMessageQueueRunning = false;
		
		Application.LoadLevel ("RockyCrag");
	}
	
	public void playerJoinedLobby()
	{
		playersInLobby++;
	}
	
	public void playerLeftLobby()
	{
		playersInLobby--;
	}
	
	void Update ()
	{
		if(playersInLobby == maxNumberOfPlayers && autoStart)
		{
			//start a count-down or load immediately?
			//either way, load the rocky crag scene, with players in the same order.
			networkView.RPC ("loadGame",RPCMode.AllBuffered);
		}
	}
}
