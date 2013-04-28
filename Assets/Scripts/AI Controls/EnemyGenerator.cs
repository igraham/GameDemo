using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
	
	
	public GameObject enemy;
	ArrayList spawnPoints = new ArrayList ();
	//add more as needed
	public GameObject spawnPoint1;
	public GameObject spawnPoint2;
	public GameObject spawnPoint3;
	public GameObject spawnPoint4;
	public GameObject spawnPoint5;
	public float spawnTime = 10f;
	private float timer =0;
	
	// Use this for initialization
	void Start () {
		//add more as needed
		spawnPoints.Add(spawnPoint1);
		spawnPoints.Add(spawnPoint2);
		spawnPoints.Add(spawnPoint3);
		spawnPoints.Add(spawnPoint4);
		spawnPoints.Add(spawnPoint5);
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		
		if(timer >= spawnTime)
		{
			int numberSpawned = Random.Range(1,3);
			
			for(int i = 0; i < numberSpawned;i++)
			{
				int spawnPointNumber = Random.Range(1,spawnPoints.Count)-1;
				GameObject spawn = (GameObject)spawnPoints[spawnPointNumber];
				Instantiate(enemy,new Vector3(spawn.transform.position.x,
					spawn.transform.position.y,spawn.transform.position.z),Quaternion.identity);
			}
			timer =0;
		}
	
	}
}
