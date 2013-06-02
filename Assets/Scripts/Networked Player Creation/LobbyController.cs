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
				networkView.RPC ("loadGame", RPCMode.All, "RockyCrag");
			}
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		playersInLobby++;
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		playersInLobby--;
	}

	IEnumerator waitTwoFrames()
	{
		yield return 0;
		yield return 0;
	}

	IEnumerator waitOneSecond()
	{
		yield return new WaitForSeconds(1.5f);
	}
	
	[RPC]
	public void loadGame(string levelName)
	{
		Network.SetSendingEnabled(0, false);
		Network.isMessageQueueRunning = false;
		if(Network.isClient)
		{
			waitOneSecond();
		}
		Application.LoadLevel(levelName);
		waitTwoFrames();
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
	}

	void Update ()
	{
		/*if(Network.isServer && playersInLobby == maxNumberOfPlayers && autoStart)
		{//don't let clients start the game anywhere, in fact.
			//start a count-down or load immediately?
			//either way, load the rocky crag scene, with players in the same order.
			networkView.RPC ("loadGame",RPCMode.AllBuffered);
		}*/
	}
}
