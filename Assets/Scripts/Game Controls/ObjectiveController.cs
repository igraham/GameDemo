using UnityEngine;
using System.Collections;

public class ObjectiveController : MonoBehaviour {
	int enemiesKilled = 0;
	int gatheredResources = 0;
	int dronesRemaining = 10;
	public int objectiveEnemies = 5;
	public int objectiveDrones = 5;
	public int objectiveResources = 1000;
	PlayerGameState player;
	// Use this for initialization
	void Start () {
	player= GetComponent<PlayerGameState>();
	}
	
	// Update is called once per frame
	void Update () {
	//gathered resources is to be checked upon using playergamestate
	gatheredResources=player.resourcesHeld;
	//drones remaining will constanly be set by the update method
	dronesRemaining=player.playerDroneCount;
	if(enemiesKilled >= objectiveEnemies 
			&& gatheredResources>=objectiveResources 
			&& dronesRemaining>=objectiveDrones)
		{
			Application.LoadLevel("Victory");
		}
	
	}

public void KillEnemy()
	{
	 enemiesKilled++;

	}
	

}
