using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
	
	
	public GameObject enemy;
	ArrayList spawnPoints = new ArrayList ();
	public float spawnTime = 20f;
	private float timer = 0;

	void Start ()
	{
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("EnemySpawn"))
		{
			spawnPoints.Add (obj);
		}
	}
	
	void Update () {
		timer += Time.deltaTime;
		
		if(timer >= spawnTime)
		{
			for(int i = 0; i < spawnPoints.Count;i++)
			{
				Network.Instantiate (enemy,((GameObject)spawnPoints[i]).transform.position, 
					Quaternion.identity,0);
			}
			timer =0;
		}
	
	}
}
