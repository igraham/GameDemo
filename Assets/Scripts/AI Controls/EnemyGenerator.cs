using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
	
	
	public GameObject enemy;
	ArrayList spawnPoints = new ArrayList ();
	public float spawnTime = 20f;
	private float timer = 0;
	public bool canSpawn = false;
	public int numEnemiesSpawning = 1;

	void Start ()
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EnemySpawn"))
		{
			spawnPoints.Add (obj);
		}
	}
	
	private ArrayList genUniqueSpawns(int numSpawns)
	{
		ArrayList indexList = new ArrayList();
		for(int i = 0; i < spawnPoints.Count; i++)
		{
			indexList.Add(i);
		}
		ArrayList spawnList = new ArrayList();
		for(int i = 0; i < numEnemiesSpawning; i++)
		{
			int uniqueIndex = Random.Range (0, indexList.Count);
			spawnList.Add (indexList[uniqueIndex]);
			indexList.RemoveAt (uniqueIndex);
		}
		indexList = null;
		return spawnList;
	}
	
	void Update () {
		timer += Time.deltaTime;
		
		if(timer >= spawnTime)
		{
			if(canSpawn)
			{
				ArrayList spawnList = genUniqueSpawns(numEnemiesSpawning);
				for(int i = 0; i < numEnemiesSpawning;i++)
				{
					Network.Instantiate (enemy,
						((GameObject)spawnPoints[(int)spawnList[i]]).transform.position, 
						Quaternion.identity,0);
				}
			}
			timer = 0;
		}
	
	}
}
